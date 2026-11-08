using Aero.TextMessageParser.Api.Contracts;
using Aero.TextMessageParser.Api.Entities;
using Aero.TextMessageParser.Api.Extensions;
using Aero.TextMessageParser.Api.Parsers.Elements;
using Aero.TextMessageParser.Api.ValueObjects;

namespace Aero.TextMessageParser.Api.Parsers.Messages;

public abstract class MessageParser<TResult>(
    MessageHeader header,
    IReadOnlyDictionary<string, Element> elementMap)
    : IMessageHandler
    where TResult : class
{
    public string EndIdentifier => $"END{header.Identifier}";
    public string RoutingKey => header.RoutingKey;
    public string MessageName
    {
        get
        {
            string result = header.Identifier;
            if (header.SecondaryIdentifier is { Length: > 0 } sec) result += $"_{sec}";
            if (header.ChangeOfStatus is { Length: > 0 } cos) result += $"_{cos}";
            return result;
        }
    }

    private Dictionary<int, int>? _seqIndexByKey;
    private Dictionary<int, Element>? _elementByKey;
    private int[]? _mandatoryIndices;
    private int[]? _singleOnlyIndices;
    private Dictionary<int, int> SeqIndexByKey => _seqIndexByKey ??= BuildSequenceIndexByKey();
    private Dictionary<int, Element> ElementByKey => _elementByKey ??= BuildElementByKey();
    private int[] MandatoryIndices => _mandatoryIndices  ??= BuildMandatoryIndices();
    private int[] SingleOnlyIndices => _singleOnlyIndices ??= BuildSingleOnlyIndices();

    private static int ElementKey(ReadOnlySpan<char> id) => id.Length < 3 ? -1 : (id[0] * 65536) + (id[1] * 256) + id[2];

    private Dictionary<int, int> BuildSequenceIndexByKey()
    {
        var seqs = ElementSequences;
        var dict = new Dictionary<int, int>(seqs.Count);
        for (int i = 0; i < seqs.Count; i++)
            dict[ElementKey(seqs[i].ElementName.AsSpan())] = i;
        return dict;
    }

    private Dictionary<int, Element> BuildElementByKey()
    {
        var dict = new Dictionary<int, Element>(ElementSequences.Count);
        foreach (var seq in ElementSequences)
            if (elementMap.TryGetValue(seq.ElementName, out var el))
                dict[ElementKey(seq.ElementName.AsSpan())] = el;
        return dict;
    }

    private int[] BuildMandatoryIndices()
    {
        var seqs = ElementSequences;
        var result = new List<int>(seqs.Count);
        for (int i = 0; i < seqs.Count; i++)
            if (seqs[i].Requirement == ElementRequirement.Mandatory && Consts.EssentialElements.Contains(seqs[i].ElementName))
                result.Add(i);
        return [.. result];
    }

    private int[] BuildSingleOnlyIndices()
    {
        var seqs = ElementSequences;
        var result = new List<int>(seqs.Count);
        for (int i = 0; i < seqs.Count; i++)
            if (!seqs[i].IsMultiple && Consts.EssentialElements.Contains(seqs[i].ElementName))
                result.Add(i);
        return [.. result];
    }

    public ParseResult<TResult> Parse(TextMessage message)
    {
        var validationResult = ValidateMessage(message);
        if (!validationResult.IsValid)
            return ParseResult<TResult>.Failure(ParseError.ForMessage(validationResult.ErrorCode!, validationResult.Description));

        List<ParseError> warnings = [];
        List<ElementResult> results = [];
        foreach (var kv in ParseElements(validationResult.Msg.Elements, warnings))
        {
            foreach (var elementResult in kv.Value)
            {
                if (elementResult.ValidationResult is { IsValid: false } vr)
                    return ParseResult<TResult>.Failure(ParseError.ForElement(vr.ErrorCode, vr.Description, vr.Line));
            }
            results.AddRange(kv.Value);
        }

        var rule = ValidateResults(results);
        if (rule is not null)
            return ParseResult<TResult>.Failure(rule);

        return ParseResult<TResult>.Success(Handle(message.Id, [.. results]), warnings);
    }
    protected virtual ParseError? ValidateResults(IReadOnlyList<ElementResult> results) => null;

    protected abstract TResult Handle(long messageId, ElementResult[] elementResults);

    protected MessageValidationResult ValidateMessage(TextMessage textMessage)
    {
        var message = textMessage.SplitSpanMessage(ElementSequences);
        var result = new MessageValidationResult(message);

        if (!string.Equals(message.Footer.EndIdentifier, EndIdentifier, StringComparison.Ordinal))
        {
            var actualFooter = message.Footer.EndIdentifier;
            if (actualFooter.Length > 40) actualFooter = actualFooter[..40];
            result.AddError(ErrorCodes.MessageEndInvalid(MessageName),
                $"Message must end with '{EndIdentifier}' but the last line is '{actualFooter}'");
            return result;
        }

        if (message.Elements.Count == 0)
        {
            result.AddError(ErrorCodes.MessageEmpty(MessageName), "Message is empty");
            return result;
        }

        var indexByKey = SeqIndexByKey;
        Span<int> seenCounts = stackalloc int[ElementSequences.Count];

        ReadOnlyMemory<char> unknownId = default;
        foreach (var element in message.Elements)
        {
            var key = ElementKey(element.Identifier.Span);
            if (indexByKey.TryGetValue(key, out var idx))
                seenCounts[idx]++;
            else
            {
                unknownId = element.Identifier;
                break;
            }
        }

        if (!unknownId.IsEmpty)
        {
            result.AddError(ErrorCodes.UNKNOWN_ELEMENT_FOUND, $"{unknownId} Unknown element found");
            return result;
        }

        foreach (var idx in MandatoryIndices)
        {
            if (seenCounts[idx] == 0)
            {
                result.AddError(ErrorCodes.MessageMissingMandatory(MessageName),
                    $"Mandatory element '{ElementSequences[idx].ElementName}' is missing");
                return result;
            }
        }

        foreach (var idx in SingleOnlyIndices)
        {
            if (seenCounts[idx] > 1)
            {
                result.AddError(ErrorCodes.MessageDuplicate(MessageName),
                    $"Element {ElementSequences[idx].ElementName} appears {seenCounts[idx]} times but IsMultiple is false");
                return result;
            }
        }

        return result;
    }

    protected Dictionary<string, List<ElementResult>> ParseElements(
        IReadOnlyList<ElementDetail> elementDetails, List<ParseError> warnings)
    {
        var parsed = new Dictionary<string, List<ElementResult>>(ElementSequences.Count, StringComparer.Ordinal);
        var seqs = ElementSequences;
        var indexByKey = SeqIndexByKey;
        var elementByKey = ElementByKey;

        foreach (var detail in elementDetails)
        {
            var key = ElementKey(detail.Identifier.Span);

            if (!indexByKey.TryGetValue(key, out var idx)) continue;
            if (!elementByKey.TryGetValue(key, out var el)) continue;

            var seq = seqs[idx];
            if (!seq.IsMultiple && parsed.ContainsKey(seq.ElementName)) continue;

            if (!parsed.TryGetValue(seq.ElementName, out var list))
                parsed[seq.ElementName] = list = [];
            var elementResult = el.Parse(detail);
            if (elementResult.ValidationResult is { IsValid: false } failed
                && !Consts.EssentialElements.Contains(seq.ElementName))
            {
                warnings.Add(ParseError.ForElement(failed.ErrorCode, failed.Description, failed.Line));
                continue;
            }
            if (elementResult.ValidationResult is { IsValid: false })
            {
                parsed[seq.ElementName] = [elementResult];
                return parsed;
            }
            list.Add(elementResult);
        }
        return parsed;
    }
    public abstract IReadOnlyList<ElementSequence> ElementSequences { get; }
}
