using Aero.Flights.Application.Common;

namespace Aero.Flights.Application.Flights.Commands;

public sealed class UpdateDepartureFlightCommandHandler(IFlightUpdateService updateService)
    : ICommandHandler<UpdateDepartureFlightCommand, UpdateFlightResult>
{
    public Task<UpdateFlightResult> HandleAsync(UpdateDepartureFlightCommand command, CancellationToken ct) =>
        updateService.ReplaceDepartureAsync(command.Id, command.Request, ct);
}
