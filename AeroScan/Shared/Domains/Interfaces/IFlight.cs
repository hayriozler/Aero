using Domain.Aggregates.Companies;

namespace Domain.Interfaces;

public interface IFlight 
{
    string RemoteSystemId { get; }
    
    string AirlineCode { get;}
    
    string FlightNumber { get;} 
    
    DateTime ScheduledDateTime { get;}
    
    DateTime? EstimatedDateTime { get;}
    
    DateTime? ActualDateTime { get;}
    
    string FlightIataDate { get;} 
    
    string IntDom { get;} 
    
    string? Terminal { get;} 
    
    string? Registration { get;} 
    
    string? ParkingPosition { get; }
    
    int TotalPassengers { get;}
    
    string? HandlingCompanyCode { get; }

    Company? HandlingCompany { get; }

    void SetHandlingCompany(string handlingCode);
    void SetFlightData(string airlineCode, string flightNumber, DateTime scheduledDateTime);
    void SetRemoteSystemId(string remoteSystemId);
    void SetOperationsDateTime(DateTime scheduledDeparture, DateTime? estimatedDateTime);
    void SetParkingPosition(string parkingPosition);
    void SetRegistration(string registration);
    void SetIntDom(string intdom);
    void SetTerminal(string terminal);
    void SetActualDateTime(DateTime actualDateTime);

}
