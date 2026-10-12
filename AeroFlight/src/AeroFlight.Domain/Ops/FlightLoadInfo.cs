namespace AeroFlight.Domain.Ops;

// Owned by Flight (see Flight.LoadInfo), mapped to its own table — kept separate purely to keep
// Flight's own property list to the fields that actually identify/describe the flight, not its
// passenger/cargo load. Always present (never null); a flight with no load data yet just has every
// field null here.
public sealed class FlightLoadInfo
{
    public int? PaxTotal { get; private set; }
    public int? PaxTransit { get; private set; }
    public int? PaxAssistanceRequired { get; private set; }
    public int? Pax1 { get; private set; }
    public int? Pax2 { get; private set; }
    public int? Pax3 { get; private set; }
    public int? CrewNumber { get; private set; }
    public int? CrewCockpit { get; private set; }
    public decimal? BaggageWeight { get; private set; }
    public decimal? BaggageTotal { get; private set; }
    public decimal? CargoWeight { get; private set; }
    public decimal? MailWeight { get; private set; }
    public decimal? TotalWeight { get; private set; }
    public decimal? EquipmentWeight { get; private set; }
    public decimal? CabinLuggage { get; private set; }

    internal FlightLoadInfo() { }

    public void UpdateCounts(int? total, int? transit, int? assistanceRequired,
        int? pax1, int? pax2, int? pax3, int? crewNumber, int? crewCockpit)
    {
        if (PaxTotal == total && PaxTransit == transit && PaxAssistanceRequired == assistanceRequired
            && Pax1 == pax1 && Pax2 == pax2 && Pax3 == pax3 && CrewNumber == crewNumber && CrewCockpit == crewCockpit)
            return;

        PaxTotal = total;
        PaxTransit = transit;
        PaxAssistanceRequired = assistanceRequired;
        Pax1 = pax1;
        Pax2 = pax2;
        Pax3 = pax3;
        CrewNumber = crewNumber;
        CrewCockpit = crewCockpit;
    }

    public void UpdateWeights(decimal? baggageWeight, decimal? baggageTotal, decimal? cargoWeight,
        decimal? mailWeight, decimal? totalWeight, decimal? equipmentWeight, decimal? cabinLuggage)
    {
        if (BaggageWeight == baggageWeight && BaggageTotal == baggageTotal && CargoWeight == cargoWeight
            && MailWeight == mailWeight && TotalWeight == totalWeight && EquipmentWeight == equipmentWeight
            && CabinLuggage == cabinLuggage)
            return;

        BaggageWeight = baggageWeight;
        BaggageTotal = baggageTotal;
        CargoWeight = cargoWeight;
        MailWeight = mailWeight;
        TotalWeight = totalWeight;
        EquipmentWeight = equipmentWeight;
        CabinLuggage = cabinLuggage;
    }
}
