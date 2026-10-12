using AeroFlight.Application.Common;

namespace AeroFlight.Application.Flights.Commands;

public sealed class UpdateDepartureFlightCommandHandler(IFlightUpdateService updateService)
    : ICommandHandler<UpdateDepartureFlightCommand, UpdateFlightResult>
{
    public Task<UpdateFlightResult> HandleAsync(UpdateDepartureFlightCommand command, CancellationToken ct) =>
        updateService.ReplaceDepartureAsync(command.Id, command.Request, ct);
}
