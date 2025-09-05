using FluentValidation;

namespace MusicManagementDemo.Application.UseCase.Identity.Login;

internal sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(e => e.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);
        RuleFor(e => e.Password).Cascade(CascadeMode.Stop).NotEmpty().MaximumLength(256);
    }
}
