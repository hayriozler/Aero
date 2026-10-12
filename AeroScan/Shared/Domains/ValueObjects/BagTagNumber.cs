namespace Domain.ValueObjects;

/// <summary>
/// IATA 10-digit bag tag: 3-char airline code + 6-digit serial + 1 Luhn check digit.
/// </summary>
public readonly record struct BagTagNumber
{
    public string Value { get; }

    private BagTagNumber(string value) => Value = value;

    public string AirlineCode => Value[..3];
    public string SerialNumber => Value[3..9];
    public char CheckDigit => Value[9];

    public static bool TryParse(string? input, out BagTagNumber result)
    {
        result = default;
        if (string.IsNullOrWhiteSpace(input) || input.Length != 10) return false;
        if (!input.All(char.IsLetterOrDigit)) return false;
        if (!LuhnValidate(input))  return false;

        result = new BagTagNumber(input.ToUpperInvariant());
        return true;
    }

    public static BagTagNumber Parse(string input)
    {
        if (!TryParse(input, out var result))
            throw new ArgumentException($"'{input}' is not a valid IATA bag tag number.", nameof(input));
        return result;
    }

    public override string ToString() => Value;

    private static bool LuhnValidate(string value)
    {
        // Standard Luhn algorithm over the numeric portion (positions 3-9) + check digit
        Span<int> digits = stackalloc int[value.Length];
        for (int i = 0; i < value.Length; i++)
        {
            if (!char.IsDigit(value[i])) return false;
            digits[i] = value[i] - '0';
        }

        int sum = 0;
        bool doubleIt = false;
        for (int i = digits.Length - 1; i >= 0; i--)
        {
            int d = digits[i];
            if (doubleIt)
            {
                d *= 2;
                if (d > 9) d -= 9;
            }
            sum += d;
            doubleIt = !doubleIt;
        }
        return sum % 10 == 0;
    }
}
