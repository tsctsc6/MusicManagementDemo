using FluentValidation;

namespace MusicManagementDemo.Application.UseCase.Identity.Register;

internal sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(c => c.Email).Cascade(CascadeMode.Stop).NotEmpty().EmailAddress();
        RuleFor(c => c.UserName).NotEmpty();
        // ASP.NET Identity 会检查密码。
        RuleFor(c => c.Password).NotEmpty();
    }
}
