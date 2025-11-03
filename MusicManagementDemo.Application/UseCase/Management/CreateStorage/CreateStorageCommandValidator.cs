using FluentValidation;

namespace MusicManagementDemo.Application.UseCase.Management.CreateStorage;

[RegisterScoped<IValidator<CreateStorageCommand>>(Duplicate = DuplicateStrategy.Append)]
internal sealed class CreateStorageCommandValidator : AbstractValidator<CreateStorageCommand>
{
    public CreateStorageCommandValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(e => e.Name).NotEmpty().MaximumLength(50);

        RuleFor(e => e.Path).NotEmpty().MaximumLength(256);
    }
}
