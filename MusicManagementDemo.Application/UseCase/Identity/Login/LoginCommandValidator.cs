using FluentValidation;

namespace MusicManagementDemo.Application.UseCase.Identity.Login;

[RegisterScoped<IValidator<LoginCommand>>(Duplicate = DuplicateStrategy.Append)]
internal sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(e => e.Email).NotEmpty().EmailAddress().MaximumLength(256);

        RuleFor(e => e.Password).NotEmpty().MaximumLength(256);
    }
}
