using FluentValidation;

namespace MusicManagementDemo.Application.UseCase.Music.CreateMusicList;

internal sealed class CreateMusicListCommandValidator : AbstractValidator<CreateMusicListCommand>
{
    public CreateMusicListCommandValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(e => e.UserId).NotNull();

        RuleFor(e => e.Name).NotEmpty().MaximumLength(100);
    }
}
