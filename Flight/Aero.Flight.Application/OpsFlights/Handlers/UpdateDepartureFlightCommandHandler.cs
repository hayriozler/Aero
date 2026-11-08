using Aero.Flights.Application.Common;
using Aero.Flights.Application.OpsFlights.Commands;

namespace Aero.Flights.Application.OpsFlights.Handlers;

public sealed class UpdateDepartureFlightCommandHandler(IFlightUpdateService updateService)
    : ICommandHandler<UpdateDepartureFlightCommand, UpdateFlightResult>
{
    public Task<UpdateFlightResult> HandleAsync(UpdateDepartureFlightCommand command, CancellationToken ct) =>
        updateService.ReplaceDepartureAsync(command.Id, command.Request, ct);
}
