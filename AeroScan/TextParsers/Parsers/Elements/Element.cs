
using IataText.Parser.Contracts;

namespace IataText.Parser.Parsers.Elements;

public abstract class Element(string name)
{
    public string ElementName => name;
    public abstract ElementResult Parse(ElementDetail detail);

    protected static int  GetAirlineLen(char c)=> char.IsDigit(c) ? 2 : 3;
}
