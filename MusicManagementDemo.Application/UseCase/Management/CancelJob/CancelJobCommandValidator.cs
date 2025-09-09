using FluentValidation;

namespace MusicManagementDemo.Application.UseCase.Management.CancelJob;

public class CancelJobCommandValidator : AbstractValidator<CancelJobCommand>
{
    public CancelJobCommandValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(e => e.JobId).NotNull().GreaterThan(0);
    }
}
