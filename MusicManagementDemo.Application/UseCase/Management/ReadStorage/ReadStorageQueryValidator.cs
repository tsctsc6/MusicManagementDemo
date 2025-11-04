using FluentValidation;

namespace MusicManagementDemo.Application.UseCase.Management.ReadStorage;

[RegisterScoped<IValidator<ReadStorageQuery>>(
    Duplicate = DuplicateStrategy.Append,
    Tags = "Validator"
)]
internal sealed class ReadStorageQueryValidator : AbstractValidator<ReadStorageQuery>
{
    public ReadStorageQueryValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(e => e.Id).NotNull().GreaterThan(0);
    }
}
