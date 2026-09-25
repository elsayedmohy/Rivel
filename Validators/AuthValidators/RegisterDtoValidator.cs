namespace RiverLine.Api.Validators.AuthValidators;

public class RegisterDtoValidator : AbstractValidator<RegisterDto>
{
    public RegisterDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MaximumLength(100)
            .WithMessage("Name must not exceed 100 characters.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Email must be a valid email address.")
            .MaximumLength(256)
            .WithMessage("Email must not exceed 256 characters.");

        RuleFor(x => x.Password)
            .StrongPassword();

        RuleFor(x => x.Role)
            .IsInEnum()
            .WithMessage("Invalid user role.");

        RuleFor(x => x.CompanyName)
            .NotEmpty()
            .WithMessage("Company name is required for carriers.")
            .MaximumLength(200)
            .WithMessage("Company name must not exceed 200 characters.")
            .When(x => x.Role == UserRole.Carrier);
    }
}