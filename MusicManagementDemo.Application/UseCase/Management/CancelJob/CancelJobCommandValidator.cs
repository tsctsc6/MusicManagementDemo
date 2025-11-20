using FluentValidation;

namespace MusicManagementDemo.Application.UseCase.Management.CancelJob;

[RegisterScoped<IValidator<CancelJobCommand>>(
    Duplicate = DuplicateStrategy.Append,
    Tags = InjectioTags.Validator
)]
public class CancelJobCommandValidator : AbstractValidator<CancelJobCommand>
{
    public CancelJobCommandValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(e => e.JobId).NotNull().GreaterThan(0);
    }
}
