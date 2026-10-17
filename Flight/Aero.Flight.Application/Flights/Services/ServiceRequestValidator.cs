using Aero.Flights.Contracts.Flights;
using FluentValidation;

namespace Aero.Flights.Application.Flights.Services;

public sealed class ServiceRequestValidator : AbstractValidator<ServiceRequest>
{
    public ServiceRequestValidator()
    {
        RuleFor(x => x.ServiceCode).NotEmpty().MaximumLength(10);
        RuleFor(x => x.AmountPlan).GreaterThanOrEqualTo(0).When(x => x.AmountPlan is not null);
        RuleFor(x => x.AmountActual).GreaterThanOrEqualTo(0).When(x => x.AmountActual is not null);
        RuleFor(x => x.HandlingAgentCode).MaximumLength(10);
        RuleFor(x => x.Comment).MaximumLength(500);
    }
}
