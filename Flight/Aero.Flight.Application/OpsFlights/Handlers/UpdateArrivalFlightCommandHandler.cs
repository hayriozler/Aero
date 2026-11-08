using Aero.Flights.Application.Common;
using Aero.Flights.Application.OpsFlights.Commands;

namespace Aero.Flights.Application.OpsFlights.Handlers;

public sealed class UpdateArrivalFlightCommandHandler(IFlightUpdateService updateService)
    : ICommandHandler<UpdateArrivalFlightCommand, UpdateFlightResult>
{
    public Task<UpdateFlightResult> HandleAsync(UpdateArrivalFlightCommand command, CancellationToken ct) =>
        updateService.ReplaceArrivalAsync(command.Id, command.Request, ct);
}
