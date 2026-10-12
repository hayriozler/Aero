using Domain.Aggregates.Companies;
using Domain.Common;

namespace Domain.Aggregates.HandlingContracts;

public sealed class AirlineHandlingContract : AuditableEntity<int>
{
    private readonly List<AirlineHandlingContractFlightNumber> _flightNumbers = [];

    public string AirlineCode { get; private set; } = string.Empty;
    public string HandlingCompanyCode { get; private set; } = null!;
    public DateTime ValidFrom { get; private set; }
    public DateTime ValidTo { get; private set; }
    public bool IsActive { get; private set; }
    public string? Notes { get; private set; }
    public Company HandlingCompany { get; private set; } = null!;
    public IReadOnlyList<AirlineHandlingContractFlightNumber> FlightNumbers => _flightNumbers.AsReadOnly();

    /// <summary>True when this contract targets specific flight numbers; false means it covers all flights of the airline.</summary>
    public bool IsSpecific => _flightNumbers.Count > 0;

    // EF Core
    private AirlineHandlingContract() { }

    public static AirlineHandlingContract Create(
        string airlineCode,
        string handlingCompanyCode,
        DateTime validFrom,
        DateTime validTo,
        string? notes = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(airlineCode);
        if (validTo <= validFrom)
            throw new ArgumentException("ValidTo must be after ValidFrom.");

        return new AirlineHandlingContract
        {
            AirlineCode       = airlineCode.ToUpperInvariant().Trim(),
            HandlingCompanyCode = handlingCompanyCode,
            ValidFrom         = validFrom,
            ValidTo           = validTo,
            IsActive          = true,
            Notes             = notes?.Trim()
        };
    }

    public void Update(
        string airlineCode,
        string handlingCompanyCode,
        DateTime validFrom,
        DateTime validTo,
        string? notes)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(airlineCode);
        if (validTo <= validFrom)
            throw new ArgumentException("ValidTo must be after ValidFrom.");

        AirlineCode       = airlineCode.ToUpperInvariant().Trim();
        HandlingCompanyCode = handlingCompanyCode;
        ValidFrom         = validFrom;
        ValidTo           = validTo;
        Notes             = notes?.Trim();
        UpdatedAt         = DateTime.UtcNow;
    }

    public void SetFlightNumbers(IEnumerable<string> flightNumbers)
    {
        _flightNumbers.Clear();
        foreach (var raw in flightNumbers)
        {
            var normalized = raw.ToUpperInvariant().Trim();
            if (!string.IsNullOrWhiteSpace(normalized) &&
                _flightNumbers.All(fn => fn.FlightNumber != normalized))
            {
                _flightNumbers.Add(AirlineHandlingContractFlightNumber.Create(Id, normalized));
            }
        }
        UpdatedAt = DateTime.UtcNow;
    }

    public bool Matches(string flightNumber, DateTime scheduledDeparture)
    {
        if (!IsActive) return false;
        if (scheduledDeparture < ValidFrom || scheduledDeparture > ValidTo) return false;
        if (_flightNumbers.Count == 0) return true;
        return _flightNumbers.Any(fn =>
            fn.FlightNumber.Equals(flightNumber, StringComparison.OrdinalIgnoreCase));
    }
}
