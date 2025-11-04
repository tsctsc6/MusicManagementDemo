using FluentValidation;

namespace MusicManagementDemo.Application.UseCase.Identity.Logout;

[RegisterScoped<IValidator<LogoutCommand>>(Duplicate = DuplicateStrategy.Append, Tags = "Validator")]
internal sealed class LogoutCommandValidator : AbstractValidator<LogoutCommand>
{
    public LogoutCommandValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.UserId).NotNull();
    }
}
