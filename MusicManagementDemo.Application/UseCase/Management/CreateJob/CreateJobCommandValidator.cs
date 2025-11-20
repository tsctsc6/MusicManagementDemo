using FluentValidation;

namespace MusicManagementDemo.Application.UseCase.Management.CreateJob;

[RegisterScoped<IValidator<CreateJobCommand>>(
    Duplicate = DuplicateStrategy.Append,
    Tags = InjectioTags.Validator
)]
internal sealed class CreateJobCommandValidator : AbstractValidator<CreateJobCommand>
{
    public CreateJobCommandValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(e => e.Type).NotNull();

        RuleFor(e => e.Description).NotEmpty().MaximumLength(500);

        RuleFor(e => e.JobArgs).NotNull();
    }
}
