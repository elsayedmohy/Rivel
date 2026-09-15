namespace RiverLine.Api.Validations.ShipmentValidations;

public sealed class CreateShipmentRequestValidator 
    : AbstractValidator<CreateShipmentRequestDto>
{
    public CreateShipmentRequestValidator()
    {
        RuleFor(x => x.CargoType)
            .NotEmpty()
            .WithMessage("Cargo type is required.")
            .MaximumLength(100)
            .WithMessage("Cargo type must not exceed 100 characters.");

        RuleFor(x => x.Weight)
            .GreaterThan(0)
            .WithMessage("Weight must be greater than 0.");

        RuleFor(x => x.Origin)
            .NotEmpty()
            .WithMessage("Origin is required.")
            .MaximumLength(200)
            .WithMessage("Origin must not exceed 200 characters.");

        RuleFor(x => x.Destination)
            .NotEmpty()
            .WithMessage("Destination is required.")
            .MaximumLength(200)
            .WithMessage("Destination must not exceed 200 characters.");

        RuleFor(x => x)
            .Must(x => !string.Equals(
                x.Origin.Trim(),
                x.Destination.Trim(),
                StringComparison.OrdinalIgnoreCase))
            .WithMessage("Origin and destination must be different.");

        RuleFor(x => x.RequestedDate)
            .NotEmpty()
            .WithMessage("Requested date is required.")
            .GreaterThanOrEqualTo(DateTime.UtcNow.Date)
            .WithMessage("Requested date cannot be in the past.");
    }
}