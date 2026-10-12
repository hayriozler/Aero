using AeroFlight.Application.Common;

namespace AeroFlight.Application.Flights.Commands;

public sealed class UpdateArrivalFlightCommandHandler(IFlightUpdateService updateService)
    : ICommandHandler<UpdateArrivalFlightCommand, UpdateFlightResult>
{
    public Task<UpdateFlightResult> HandleAsync(UpdateArrivalFlightCommand command, CancellationToken ct) =>
        updateService.ReplaceArrivalAsync(command.Id, command.Request, ct);
}
