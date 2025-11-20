using FluentValidation;

namespace MusicManagementDemo.Application.UseCase.Management.DeleteStorage;

[RegisterScoped<IValidator<DeleteStorageCommand>>(
    Duplicate = DuplicateStrategy.Append,
    Tags = InjectioTags.Validator
)]
internal sealed class DeleteStorageCommandValidator : AbstractValidator<DeleteStorageCommand>
{
    public DeleteStorageCommandValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(e => e.Id).NotNull().GreaterThan(0);
    }
}
