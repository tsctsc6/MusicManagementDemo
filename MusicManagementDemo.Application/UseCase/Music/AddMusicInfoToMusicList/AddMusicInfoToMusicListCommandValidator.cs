using FluentValidation;

namespace MusicManagementDemo.Application.UseCase.Music.AddMusicInfoToMusicList;

internal sealed class AddMusicInfoToMusicListCommandValidator
    : AbstractValidator<AddMusicInfoToMusicListCommand>
{
    public AddMusicInfoToMusicListCommandValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(e => e.UserId).NotEmpty().MaximumLength(36);

        RuleFor(e => e.MusicListId).NotNull();

        RuleFor(e => e.MusicInfoId).NotNull();
    }
}
