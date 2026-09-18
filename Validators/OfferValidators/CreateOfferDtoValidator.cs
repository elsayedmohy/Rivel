namespace RiverLine.Api.Validators.OfferValidators;

public sealed class CreateOfferDtoValidator : AbstractValidator<CreateOfferDto>
{
    public CreateOfferDtoValidator()
    {
        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("Price must be greater than 0.");

        RuleFor(x => x.ProposedPickupDate)
            .NotEmpty()
            .WithMessage("Proposed pickup date is required.")
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("Proposed pickup date cannot be in the past.");

        RuleFor(x => x.VesselId)
            .NotEmpty()
            .WithMessage("Vessel is required.");
    }
}