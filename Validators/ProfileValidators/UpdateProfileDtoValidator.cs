namespace RiverLine.Api.Validators.ProfileValidators;

public class UpdateProfileDtoValidator : AbstractValidator<UpdateProfileDto>
{
    public UpdateProfileDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MaximumLength(100)
            .WithMessage("Name must not exceed 100 characters.");

        RuleFor(x => x.PhoneNumber)
            .Must(PhoneNumbers.IsValid)
            .WithMessage("Phone number must be 7–15 digits, optionally starting with +.");

        // إلزامية اسم الشركة للناقل بتتشيك في السيرفس — الـ validator مايعرفش دور المستخدم.
        RuleFor(x => x.CompanyName)
            .MaximumLength(200)
            .WithMessage("Company name must not exceed 200 characters.");

        RuleFor(x => x.Bio)
            .MaximumLength(1000)
            .WithMessage("Bio must not exceed 1000 characters.");
    }
}
