using Aero.Flights.Contracts.Flights;
using FluentValidation;

namespace Aero.Flights.Application.Flights.Resources;

public sealed class FlightResourceRequestValidator : AbstractValidator<FlightResourceRequest>
{
    public FlightResourceRequestValidator()
    {
        // Code is optional — operators add a placeholder resource first and assign the real code later.
        RuleFor(x => x.Code).MaximumLength(20);
        RuleFor(x => x.CheckinClassCode).MaximumLength(10);
    }
}

public sealed class BulkResourceStatusRequestValidator : AbstractValidator<BulkResourceStatusRequest>
{
    public BulkResourceStatusRequestValidator()
    {
        RuleFor(x => x.StatusCode).NotEmpty().MaximumLength(5);
    }
}
