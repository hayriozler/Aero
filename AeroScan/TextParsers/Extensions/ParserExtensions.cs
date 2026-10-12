using IataText.Parser.Contracts;
using IataText.Parser.Entities;
using IataText.Parser.Parsers.Elements;
using IataText.Parser.ValueObjects;

namespace IataText.Parser.Extensions;

public static class ParserExtensions
{
    private static List<ReadOnlyMemory<char>> SplitText(this string text, char[] seperator)
    {
        var span = text.AsMemory();
        List<ReadOnlyMemory<char>> result = [];
        foreach (var part in span.Span.Split(seperator))
        {
            var value = span[part.Start.Value..part.End.Value];
            if (!value.IsEmpty)
            {
                result.Add(value);
            }
        }
        return result;
    }
    public static (MessageHeader messageHeader, MessageFooter endIdentifier) GetMessageIdentifier(this string text)
    {
        var splittedText = SplitText(text, Consts.NEW_LINE_SEPERATOR);
        string identifier = splittedText[0].ToString();
        string? secondaryIdentifier = null;
        string? changeOfStatus = null;
        if (splittedText[1].Length >= 3)
        {
            string? secondLine = splittedText[1][..3].ToString();
            if (Consts.CHANGE_OF_STATUS.Contains(secondLine))
            {
                changeOfStatus = secondLine;
            }
            else
            {
                if (secondLine[0] != '.')
                    secondaryIdentifier = secondLine[0..3].ToString();
            }
        }
        var len = splittedText.Count - 1;
        var endIdentifier = splittedText[len];
        return (new MessageHeader(identifier, secondaryIdentifier, changeOfStatus), new(endIdentifier.ToString()));
    }
    public static Message SplitSpanMessage(this TextMessage textMessage, IReadOnlyList<ElementSequence> elementSequences)
    {
        var mem        = textMessage.Message.AsMemory();
        var headerSpan = textMessage.Header.Identifier.AsSpan();
        var footerSpan = textMessage.Footer.EndIdentifier.AsSpan();

        ReadOnlySpan<char> secSpan = textMessage.Header.SecondaryIdentifier is { } s
            ? s.AsSpan() : default;
        ReadOnlySpan<char> cosSpan = textMessage.Header.ChangeOfStatus is { } c
            ? c.AsSpan() : default;

        var elements = new List<ElementDetail>(24);
        
        foreach (var lineRange in mem.Span.Split(Consts.NEW_LINE_SEPERATOR))
        {
            var lineMem  = mem[lineRange];
            var lineSpan = lineMem.Span;

            if (lineSpan.IsEmpty)                         continue;
            if (lineSpan.SequenceEqual(headerSpan))       continue;
            if (!secSpan.IsEmpty && lineSpan.SequenceEqual(secSpan)) continue;
            if (!cosSpan.IsEmpty && lineSpan.SequenceEqual(cosSpan)) continue;
            if (lineSpan.SequenceEqual(footerSpan))       continue;

            var identifier = lineMem[..Math.Min(3, lineMem.Length)];
            var elementDetail = elementSequences.First(p => p.ElementName.AsSpan().SequenceEqual(identifier.Span));
            var fields = SplitOnSlash(lineMem);
            elements.Add(new ElementDetail(identifier, lineMem, fields, 2 + lineMem.Length, elementDetail.Requirement));
        }

        return new Message(textMessage.Header, elements.AsReadOnly(), textMessage.Footer);
    }

    public static ReadOnlyMemory<char>[] SplitOnSlash(ReadOnlyMemory<char> line)
    {
        int slashCount = 0;
        foreach (var c in line.Span)
            if (c == '/') slashCount++;

        var result = new ReadOnlyMemory<char>[slashCount + 1];
        int idx = 0;
        foreach (var range in line.Span.Split('/'))
        {
            var field = line[range];
            result[idx++] = field;
        }
        return result;
    }

    public static bool ContainsSpan(this string[] values, ReadOnlySpan<char> span)
    {
        foreach (var v in values)
            if (span.SequenceEqual(v.AsSpan())) return true;
        return false;
    }

    public static bool ContainsChar(this char[] values, char c)
    {
        foreach (var v in values)
            if (v == c) return true;
        return false;
    }
}
