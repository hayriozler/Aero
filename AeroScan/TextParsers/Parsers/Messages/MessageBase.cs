using IataText.Parser.Contracts;
using IataText.Parser.Entities;
using IataText.Parser.Extensions;
using IataText.Parser.Parsers.Elements;
using IataText.Parser.Persistence;
using IataText.Parser.ValueObjects;

namespace IataText.Parser.Parsers.Messages;

public abstract class MessageBase<TResult>(
    MessageHeader header,
    IReadOnlyDictionary<string, Element> elementMap)
    : IMessageHandler
{
    public string EndIdentifier => $"END{header.Identifier}";
    public string MessageName
    {
        get
        {
            string result = header.Identifier;
            if (header.SecondaryIdentifier is { Length: > 0 } sec) result += $"_{sec}";
            if (header.ChangeOfStatus      is { Length: > 0 } cos) result += $"_{cos}";
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

    private static int ElementKey(ReadOnlySpan<char> id) => (id[0] * 65536) + (id[1] * 256) + id[2];

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
            if (seqs[i].Requirement == ElementRequirement.Mandatory)
                result.Add(i);
        return [.. result];
    }

    private int[] BuildSingleOnlyIndices()
    {
        var seqs = ElementSequences;
        var result = new List<int>(seqs.Count);
        for (int i = 0; i < seqs.Count; i++)
            if (!seqs[i].IsMultiple)
                result.Add(i);
        return [.. result];
    }

    private (bool IsValid, ElementResult[] ElementResults) ValidateAndCollectErrors(
        TextMessage message, ITextParserDbContext dbContext)
    {
        var validationResult = ValidateMessage(message);
        if (!validationResult.IsValid)
        {
            dbContext.TextMessageErrorSet.Add(
                TextMessageError.Create(message.Id, validationResult.ErrorCode!, validationResult.Description));
            return (false, []);
        }

        List<ElementResult> results = [];
        var elementResults = ParseElements(validationResult.Msg.Elements);
        foreach (var kv in elementResults)
        {
            foreach (var er in kv.Value)
            {
                if (er.ValidationResult is { IsValid: false } vr)
                {
                    dbContext.TextMessageElementErrorSet.Add(
                        ElementError.Create(message.Id, vr.ErrorCode, vr.Description));
                    return (false, []);
                }
            }
            results.AddRange(kv.Value);
        }

        return (true, [.. results]);
    }
    protected abstract TResult Handle(long messageId, ElementResult[] elementResults);

    protected MessageValidationResult ValidateMessage(TextMessage textMessage)
    {
        var message = textMessage.SplitSpanMessage(ElementSequences);
        var result = new MessageValidationResult(message);

        if (message.Elements.Count == 0)
        {
            result.AddError($"MSG_{MessageName}_EMPTY", "Message is empty");
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
            result.AddError("UNKNOWN_ELEMENT_FOUND", $"{unknownId} Unknown element found");
            return result;
        }

        foreach (var idx in MandatoryIndices)
        {
            if (seenCounts[idx] == 0)
            {
                result.AddError($"MSG_{MessageName}_MISSING_MANDATORY",
                    $"Mandatory element '{ElementSequences[idx].ElementName}' is missing");
                return result;
            }
        }

        foreach (var idx in SingleOnlyIndices)
        {
            if (seenCounts[idx] > 1)
            {
                result.AddError($"MSG_{MessageName}_DUPLICATE",
                    $"Element {ElementSequences[idx].ElementName} appears {seenCounts[idx]} times but IsMultiple is false");
                return result;
            }
        }

        return result;
    }

    protected Dictionary<string, List<ElementResult>> ParseElements(
        IReadOnlyList<ElementDetail> elementDetails)
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
    public TResult? ProcessMessage(TextMessage message, ITextParserDbContext dbContext)
    {
        var (isValid, elementResults) = ValidateAndCollectErrors(message, dbContext);
        if (!isValid) return default;
        return Handle(message.Id, elementResults);
    }
}
