using FluentValidation;

namespace MusicManagementDemo.Application.UseCase.Music.AddMusicInfoToMusicList;

internal sealed class AddMusicInfoToMusicListCommandValidator
    : AbstractValidator<AddMusicInfoToMusicListCommand>
{
    public AddMusicInfoToMusicListCommandValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(e => e.UserId).NotNull();

        RuleFor(e => e.MusicListId).NotNull();

        RuleFor(e => e.MusicInfoId).NotNull();
    }
}
