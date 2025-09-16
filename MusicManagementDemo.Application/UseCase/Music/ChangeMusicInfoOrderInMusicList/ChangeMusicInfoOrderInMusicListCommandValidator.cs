using FluentValidation;

namespace MusicManagementDemo.Application.UseCase.Music.ChangeMusicInfoOrderInMusicList;

internal sealed class ChangeMusicInfoOrderInMusicListCommandValidator
    : AbstractValidator<ChangeMusicInfoOrderInMusicListCommand>
{
    public ChangeMusicInfoOrderInMusicListCommandValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.UserId).NotEmpty().MaximumLength(36);

        RuleFor(x => x.MusicListId).NotNull();

        RuleFor(x => x.TargetMusicInfoId).NotNull();

        RuleFor(x => x.PrevMusicInfoId);

        RuleFor(x => x.NextMusicInfoId);
    }
}
