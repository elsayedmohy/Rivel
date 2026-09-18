
namespace RiverLine.Api.Validators.RatingValidators;


public sealed class CreateRatingDtoValidator : AbstractValidator<CreateRatingDto>
{
    public CreateRatingDtoValidator()
    {
        RuleFor(x => x.ShipmentId)
            .NotEmpty()
            .WithMessage("Shipment Id is required.");
        
        RuleFor(x => x.Score)
            .InclusiveBetween(1, 5)
            .WithMessage("Score must be between 1 and 5.");

        RuleFor(x => x.Comment)
            .MaximumLength(1000)
            .WithMessage("Comment must not exceed 1000 characters.");
    }
}
