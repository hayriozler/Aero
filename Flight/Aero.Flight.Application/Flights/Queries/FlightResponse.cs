using Aero.Flights.Contracts.Flights;
using Aero.Flights.Domain.Ops;

namespace Aero.Flights.Application.Flights.Queries;

public static class FlightResponseFactory
{
    public static FlightResponse FromArrival(ArrivalFlight flight)
        => new(flight.Id, "Arrival", flight.AirlineCode, flight.FlightNumber, flight.Sta, flight.Status);

    public static FlightResponse FromDeparture(DepartureFlight flight)
        => new(flight.Id, "Departure", flight.AirlineCode, flight.FlightNumber, flight.Std, flight.Status);
}
