using FluentValidation;

namespace MusicManagementDemo.Application.UseCase.Management.UpdateStorage;

internal sealed class UpdateStorageCommandValidator : AbstractValidator<UpdateStorageCommand>
{
    public UpdateStorageCommandValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(e => e.Id).NotNull().GreaterThan(0);

        RuleFor(e => e.Name).NotEmpty().MaximumLength(50);

        RuleFor(e => e.Path).NotEmpty().MaximumLength(256);
    }
}
