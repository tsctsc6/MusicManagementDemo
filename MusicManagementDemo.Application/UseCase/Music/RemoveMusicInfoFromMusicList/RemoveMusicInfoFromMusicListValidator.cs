using FluentValidation;

namespace MusicManagementDemo.Application.UseCase.Music.RemoveMusicInfoFromMusicList;

internal sealed class RemoveMusicInfoFromMusicListValidator
    : AbstractValidator<RemoveMusicInfoFromMusicListCommand>
{
    public RemoveMusicInfoFromMusicListValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(e => e.UserId).NotEmpty().MaximumLength(36);

        RuleFor(e => e.MusicListId).NotNull();

        RuleFor(e => e.MusicListId).NotNull();
    }
}
