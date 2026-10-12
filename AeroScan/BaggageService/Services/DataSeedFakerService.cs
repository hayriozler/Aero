using BaggageService.Persistence;
using Bogus;
using Domain.Aggregates.Flights;
using Domain.Aggregates.Reconciliations;
using Domain.Enums;

namespace BaggageService.Services;

public class DataSeedFakerService(AeroScanDataContext dataContext)
{
    private static readonly string[] _airlineCodes =
        ["KL", "BA", "LH", "AF", "EK", "QR", "TK", "UA", "DL", "AA", "AC", "IB", "AZ", "OS", "SN", "SK", "AY", "LO", "LX", "TP", "EI", "FR", "U2"];

    private static readonly string[] _airports =
        ["JFK", "LHR", "FRA", "CDG", "DXB", "DOH", "IST", "EWR", "ATL", "ORD", "YYZ", "MAD", "FCO", "VIE", "BRU", "CPH", "HEL", "WAW", "ZRH", "LIS", "DUB", "BCN", "BER"];

    private static readonly string[] _handlingCodes = ["Tgs", "Havas", "Celebi"];

    // Weighted pool — Scheduled and Departed appear more often than edge states
    private static readonly DepartureFlightStatus[] _departureStatusPool =
    [
        DepartureFlightStatus.Scheduled,
        DepartureFlightStatus.CheckInOpen, 
        DepartureFlightStatus.Boarding,
        DepartureFlightStatus.FinalCall,
        DepartureFlightStatus.Departed, 
        DepartureFlightStatus.Cancelled
    ];

    private static readonly ArrivalFlightStatus[] _arrivalStatusPool =
    [
        ArrivalFlightStatus.Scheduled,
        ArrivalFlightStatus.Estimated,
        ArrivalFlightStatus.Arrived, 
        ArrivalFlightStatus.Cancelled
    ];

    private List<DepartureFlight> BuildDepartureFlights(Faker f, int count)
    {
        var result = new List<DepartureFlight>(count);
        for (var i = 0; i < count; i++)
        {
            var airline     = f.PickRandom(_airlineCodes);
            var destination = f.PickRandom(_airports);
            var nextAirport = f.Random.Bool(0.3f)
                ? f.PickRandom(_airports.Where(a => a != destination).ToArray())
                : destination;

            var scheduled = f.Date
                .Between(DateTime.Today.AddDays(-1), DateTime.Today.AddDays(2))
                .ToUniversalTime();

            var status = f.PickRandom(_departureStatusPool);

            // EstimatedDateTime: only for non-Scheduled, non-Cancelled — always at least 25 min after STD
            DateTime? estimated = status is DepartureFlightStatus.Scheduled or DepartureFlightStatus.Cancelled
                ? null
                : scheduled.AddMinutes(f.Random.Int(25, 90));

            var flight = DepartureFlight.Create(
                $"DEP{i:D4}{airline}",
                airline,
                f.Random.Int(10, 9999).ToString(),
                nextAirport,
                destination,
                scheduled,
                estimated);

            flight.SetFlightStatus(status);

            // ActualDateTime: only when the flight has physically departed
            if (status == DepartureFlightStatus.Departed)
                flight.SetActualDateTime(scheduled.AddMinutes(f.Random.Int(25, 120)));

            // Gate and check-in are only relevant while the flight is still being processed
            if (status is not (DepartureFlightStatus.Departed or DepartureFlightStatus.Cancelled))
            {
                flight.SetGate($"G{f.Random.Int(1, 40)}", f.PickRandom("O", "B", "F", "C"));
                flight.SetCheckin($"C{f.Random.Int(1, 20)}", f.PickRandom("O", "C"));
            }

            flight.SetChute($"CH-{f.Random.Int(1, 12)}");
            flight.SetTerminal($"T{f.Random.Int(1, 4)}");
            flight.SetParkingPosition($"P{f.Random.Int(1, 99)}");
            flight.SetRegistration(
                $"{f.Random.String2(2, "ABCDEFGHIJKLMNOPQRSTUVWXYZ")}-{f.Random.String2(3, "ABCDEFGHIJKLMNOPQRSTUVWXYZ")}");
            flight.SetIntDom(f.PickRandom("I", "D"));
            flight.SetHandlingCompany(f.PickRandom(_handlingCodes));

            result.Add(flight);
        }
        return result;
    }

    private List<ArrivalFlight> BuildArrivalFlights(Faker f, int count)
    {
        var result = new List<ArrivalFlight>(count);
        for (var i = 0; i < count; i++)
        {
            var airline        = f.PickRandom(_airlineCodes);
            var origin         = f.PickRandom(_airports);
            var previousAirport = f.Random.Bool(0.3f)
                ? f.PickRandom(_airports.Where(a => a != origin).ToArray())
                : origin;

            var scheduled = f.Date
                .Between(DateTime.Today.AddDays(-1), DateTime.Today.AddDays(2))
                .ToUniversalTime();

            var status = f.PickRandom(_arrivalStatusPool);

            // EstimatedDateTime: set for Departed (en route) — at least 25 min from STA
            DateTime? estimated = status is ArrivalFlightStatus.Arrived
                ? scheduled.AddMinutes(f.Random.Int(25, 90))
                : null;

            var flight = ArrivalFlight.Create(
                $"ARR{i:D4}{airline}",
                airline,
                f.Random.Int(10, 9999).ToString(),
                previousAirport,
                origin,
                scheduled,
                estimated);

            flight.SetFlightStatus(status);

            // ActualDateTime: set once the aircraft has physically landed
            if (status == ArrivalFlightStatus.Arrived)
                flight.SetActualDateTime(scheduled.AddMinutes(f.Random.Int(25, 120)));

            flight.SetCarousel($"B{f.Random.Int(1, 12)}");
            flight.SetTerminal($"T{f.Random.Int(1, 4)}");
            flight.SetParkingPosition($"P{f.Random.Int(1, 99)}");
            flight.SetRegistration(
                $"{f.Random.String2(2, "ABCDEFGHIJKLMNOPQRSTUVWXYZ")}-{f.Random.String2(3, "ABCDEFGHIJKLMNOPQRSTUVWXYZ")}");
            flight.SetIntDom(f.PickRandom("I", "D"));
            flight.SetHandlingCompany(f.PickRandom(_handlingCodes));

            result.Add(flight);
        }
        return result;
    }

    public async Task FeedDummyDataAsync(int top)
    {
        var f = new Faker();

        var depFlights = BuildDepartureFlights(f, top);
        var arrFlights = BuildArrivalFlights(f, top);

        dataContext.DepartureFlightSet.AddRange(depFlights);
        dataContext.ArrivalFlightSet.AddRange(arrFlights);
        await dataContext.SaveChangesAsync();

        // Reconciliation records — one per active flight (UPSERT model: unique FlightId)
        foreach (var flight in depFlights)
        {
            if (flight.FlightStatus is DepartureFlightStatus.Scheduled or DepartureFlightStatus.Cancelled) continue;

            var expected = f.Random.Int(60, 280);
            var loaded   = flight.FlightStatus == DepartureFlightStatus.Departed
                ? expected - f.Random.Int(0, 4)
                : f.Random.Int(0, expected);
            var missing  = expected - loaded > 0 ? f.Random.Int(0, Math.Min(5, expected - loaded)) : 0;
            var offloaded = f.Random.Int(0, 3);
            var rush      = f.Random.Int(0, 6);
            var priority  = f.Random.Int(0, 10);
            var transfer  = f.Random.Int(0, 20);

            var rec = DepartureFlightReconciliation.Create(flight.Id);
            rec.UpdateStats(
                expected:            expected,
                loaded:              loaded,
                offloaded:           offloaded,
                toBeOffloaded:       f.Random.Int(0, 3),
                waiting:             f.Random.Int(0, expected - loaded),
                missing:             missing,
                reconciled:          loaded - missing,
                forceLoaded:         f.Random.Int(0, 2),
                onward:              f.Random.Int(0, 5),
                transferLoaded:      transfer,
                transferMissing:     f.Random.Int(0, 3),
                notBoardedPassenger: f.Random.Int(0, 5),
                rush:                rush,
                priority:            priority);

            if (flight.FlightStatus == DepartureFlightStatus.Departed)
                rec.MarkFinal();

            dataContext.DepartureFlightReconciliationSet.Add(rec);
        }

        foreach (var flight in arrFlights)
        {
            if (flight.FlightStatus is ArrivalFlightStatus.Scheduled or ArrivalFlightStatus.Cancelled) continue;

            var expected  = f.Random.Int(60, 280);
            var unloaded  = flight.FlightStatus == ArrivalFlightStatus.Arrived
                ? expected - f.Random.Int(0, 4)
                : f.Random.Int(0, expected / 2);
            var remaining = expected - unloaded;
            var delivered = flight.FlightStatus == ArrivalFlightStatus.Arrived ? unloaded - f.Random.Int(0, 10) : 0;
            var missing   = f.Random.Int(0, Math.Min(5, unloaded));
            var transfer  = f.Random.Int(0, 20);
            var rush      = f.Random.Int(0, 6);

            var rec = ArrivalFlightReconciliation.Create(flight.Id);
            rec.UpdateStats(
                expected:  expected,
                unloaded:  unloaded,
                remaining: remaining,
                toBelt:    f.Random.Int(0, unloaded),
                delivered: delivered,
                transfer:  transfer,
                missing:   missing,
                unknown:   f.Random.Int(0, 3),
                rush:      rush);

            if (flight.FlightStatus == ArrivalFlightStatus.Arrived)
                rec.MarkFinal();

            dataContext.ArrivalFlightReconciliationSet.Add(rec);
        }

        await dataContext.SaveChangesAsync();
    }
}
