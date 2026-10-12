namespace Domain.Interfaces;
public interface IBag
{
    int FlightPassengerId { get; }
    string TagNumber { get; }
    char SourceIndicator { get; }
    string? SourceAirportCode { get; }
    int? HhtId { get; }
    string? UserName { get; }
    bool IsDeleted { get; }
    void SetDelete();
   void AddEvent(IBagEvent bagEvent);

   IReadOnlyList<IBagEvent> BagEvents { get; }
}
