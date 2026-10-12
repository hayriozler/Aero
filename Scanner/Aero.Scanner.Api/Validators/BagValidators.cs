using Aero.Scanner.Api.Services;
using Aero.Scanner.Shared.Requests;
using FluentValidation;

namespace Aero.Scanner.Api.Validators;

public sealed class CheckInBagRequestValidator : AbstractValidator<CheckInBagRequest>
{
    public CheckInBagRequestValidator()
    {
        RuleFor(x => x.TagNumber)
            .NotEmpty()
            .Must(t => BagTagNumberService.TryParse(t, out _))
            .WithMessage("'{PropertyValue}' is not a valid IATA 10-digit bag tag number.");
        RuleFor(x => x.FlightKey).NotEmpty();
        RuleFor(x => x.WeightKg).GreaterThan(0);
        RuleFor(x => x.OperatorId).NotEmpty();
        RuleFor(x => x.DeviceId).NotEmpty();
        RuleFor(x => x.LocationCode).NotEmpty();
    }
}

public sealed class OffloadBagRequestValidator : AbstractValidator<OffloadBagRequest>
{
    public OffloadBagRequestValidator()
    {
        RuleFor(x => x.OperatorId).NotEmpty();
        RuleFor(x => x.DeviceId).NotEmpty();
        RuleFor(x => x.LocationCode).NotEmpty();
    }
}

public sealed class TransferBagRequestValidator : AbstractValidator<TransferBagRequest>
{
    public TransferBagRequestValidator()
    {
        RuleFor(x => x.TagNumber)
            .NotEmpty()
            .Must(t => BagTagNumberService.TryParse(t, out _))
            .WithMessage("'{PropertyValue}' is not a valid IATA 10-digit bag tag number.");
        RuleFor(x => x.TargetFlightKey).NotEmpty();
        RuleFor(x => x.OperatorId).NotEmpty();
    }
}
