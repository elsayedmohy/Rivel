namespace RiverLine.Api.Validators.ShipmentValidators;

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

        RuleFor(x => x.OriginNileBerthId)
            .NotEmpty()
            .WithMessage("Origin Port is required.")
            .WithMessage("Origin Port must not exceed 200 characters.");

        RuleFor(x => x.DestinationNileBerthId)
            .NotEmpty()
            .WithMessage("Destination Port is required.")
            .WithMessage("Destination Port must not exceed 200 characters.");

        RuleFor(x => x)
            .Must(x => !string.Equals(
                x.OriginNileBerthId.ToString().Trim(),
                x.DestinationNileBerthId.ToString().Trim(),
                StringComparison.OrdinalIgnoreCase))
            .WithMessage("OriginPort and destination must be different.");

        RuleFor(x => x.RequestedDate)
            .NotEmpty()
            .WithMessage("Requested date is required.")
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("Requested date cannot be in the past.");
    }
}