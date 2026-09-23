namespace RiverLine.Api.Validators.VesselValidators;

public class CreateVesselDtoValidator : AbstractValidator<CreateVesselDto>
{
    public CreateVesselDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Vessel name is required.")
            .MaximumLength(100)
            .WithMessage("Vessel name cannot exceed 100 characters.");

        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Invalid vessel type.");

        RuleFor(x => x.RegistrationNumber)
            .NotEmpty()
            .WithMessage("Registration number is required.")
            .MaximumLength(50)
            .WithMessage("Registration number cannot exceed 50 characters.");

        RuleFor(x => x.Capacity)
            .GreaterThan(0)
            .WithMessage("Capacity must be greater than 0.")
            .LessThanOrEqualTo(20_000);
        
        RuleFor(x => x.YearBuilt)
            .InclusiveBetween(1900, DateTime.UtcNow.Year)
            .When(x => x.YearBuilt.HasValue);
    }
}