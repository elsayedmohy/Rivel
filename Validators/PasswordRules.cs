namespace RiverLine.Api.Validators;

// سياسة كلمة السر في مكان واحد — كانت متكررة في RegisterDtoValidator،
// وكانت هتتكرر تاني في تغيير كلمة السر وفي reset-password (سلايس ٢).
// لازم تفضل مطابقة لإعدادات options.Password في AddAuthenticationServices
// ولـ strongPassword في الفرونت (features/auth/password.validator.ts).
public static class PasswordRules
{
    public static IRuleBuilderOptions<T, string> StrongPassword<T>(this IRuleBuilder<T, string> rule) =>
        rule
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one number.")
            .Matches(@"[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");
}
