using FluentValidation;

namespace MusicManagementDemo.Application.UseCase.Identity.Logout;

[RegisterScoped<IValidator<LogoutCommand>>(Duplicate = DuplicateStrategy.Append)]
internal sealed class LogoutCommandValidator : AbstractValidator<LogoutCommand>
{
    public LogoutCommandValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.UserId).NotNull();
    }
}
