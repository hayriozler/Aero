using IataText.Parser.Parsers.Elements;

namespace IataText.Parser.Contracts;

public readonly record struct ElementDetail(
    ReadOnlyMemory<char> Identifier,   // 3-char element prefix, e.g. ".V/"
    ReadOnlyMemory<char> Text,         // full element line, e.g. ".V/1LFRA/001/REF"
    ReadOnlyMemory<char>[] ParsedText,     // split on '/': [".V","1LFRA","001","REF"]
    int Length,                         // 2 + Text.Length  (matches original semantics)
    ElementRequirement Requirement
);
