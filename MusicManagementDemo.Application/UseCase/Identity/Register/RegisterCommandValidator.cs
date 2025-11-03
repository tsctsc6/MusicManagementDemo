using FluentValidation;

namespace MusicManagementDemo.Application.UseCase.Identity.Register;

[RegisterScoped<IValidator<RegisterCommand>>(Duplicate = DuplicateStrategy.Append)]
internal sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(c => c.Email).NotEmpty().EmailAddress().MaximumLength(256);

        RuleFor(c => c.UserName).NotEmpty().MaximumLength(256);

        // ASP.NET Identity 会检查密码。
        RuleFor(c => c.Password).NotEmpty().MaximumLength(256);
    }
}
