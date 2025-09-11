using FluentValidation;

namespace MusicManagementDemo.Application.UseCase.Management.ReadAllStorage;

internal sealed class ReadAllStorageQueryValidator : AbstractValidator<ReadAllStorageQuery>
{
    public ReadAllStorageQueryValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(e => e.PageSize).NotNull().GreaterThanOrEqualTo(0);

        RuleFor(e => e.PageSize).NotNull().GreaterThanOrEqualTo(5);
    }
}
