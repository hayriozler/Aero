using AeroFlight.Contracts.Flights;
using AeroFlight.Domain.Ops;

namespace AeroFlight.Application.Flights.Queries;

public static class FlightResponseFactory
{
    public static FlightResponse FromArrival(ArrivalFlight flight)
        => new(flight.Id, "Arrival", flight.AirlineCode, flight.FlightNumber, flight.Sta, flight.Status);

    public static FlightResponse FromDeparture(DepartureFlight flight)
        => new(flight.Id, "Departure", flight.AirlineCode, flight.FlightNumber, flight.Std, flight.Status);
}
