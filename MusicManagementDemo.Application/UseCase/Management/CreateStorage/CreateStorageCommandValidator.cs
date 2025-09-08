using FluentValidation;

namespace MusicManagementDemo.Application.UseCase.Management.CreateStorage;

internal sealed class CreateStorageCommandValidator : AbstractValidator<CreateStorageCommand>
{
    public CreateStorageCommandValidator()
    {
        RuleFor(e => e.Name).NotEmpty().MaximumLength(50);

        RuleFor(e => e.Path).NotEmpty().MaximumLength(256);
    }
}
