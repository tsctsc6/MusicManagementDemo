using FluentValidation;

namespace MusicManagementDemo.Application.UseCase.Identity.Register;

internal sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(c => c.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);
        RuleFor(c => c.UserName).NotEmpty().MaximumLength(256);
        // ASP.NET Identity 会检查密码。
        RuleFor(c => c.Password).NotEmpty().MaximumLength(256);
    }
}
