using FluentValidation;

namespace MusicManagementDemo.Application.UseCase.Identity.Login;

internal sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(e => e.Email).NotEmpty().EmailAddress().MaximumLength(256);

        RuleFor(e => e.Password).NotEmpty().MaximumLength(256);
    }
}
