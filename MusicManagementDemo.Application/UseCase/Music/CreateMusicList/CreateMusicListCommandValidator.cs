using FluentValidation;

namespace MusicManagementDemo.Application.UseCase.Music.CreateMusicList;

internal sealed class CreateMusicListCommandValidator : AbstractValidator<CreateMusicListCommand>
{
    public CreateMusicListCommandValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(e => e.UserId).NotEmpty().MaximumLength(36);

        RuleFor(e => e.Name).NotEmpty().MaximumLength(100);
    }
}
