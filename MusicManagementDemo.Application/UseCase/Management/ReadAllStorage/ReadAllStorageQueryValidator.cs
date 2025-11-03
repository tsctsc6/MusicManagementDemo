using FluentValidation;

namespace MusicManagementDemo.Application.UseCase.Management.ReadAllStorage;

[RegisterScoped<IValidator<ReadAllStorageQuery>>(Duplicate = DuplicateStrategy.Append)]
internal sealed class ReadAllStorageQueryValidator : AbstractValidator<ReadAllStorageQuery>
{
    public ReadAllStorageQueryValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.ReferenceId);

        RuleFor(x => x.PageSize).NotNull().GreaterThan(0).LessThan(30);

        RuleFor(x => x.Asc).NotNull();
    }
}
