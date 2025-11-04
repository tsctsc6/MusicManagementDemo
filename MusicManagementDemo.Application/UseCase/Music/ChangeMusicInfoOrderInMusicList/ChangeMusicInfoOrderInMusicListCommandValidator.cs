using FluentValidation;

namespace MusicManagementDemo.Application.UseCase.Music.ChangeMusicInfoOrderInMusicList;

[RegisterScoped<IValidator<ChangeMusicInfoOrderInMusicListCommand>>(
    Duplicate = DuplicateStrategy.Append,
    Tags = "Validator"
)]
internal sealed class ChangeMusicInfoOrderInMusicListCommandValidator
    : AbstractValidator<ChangeMusicInfoOrderInMusicListCommand>
{
    public ChangeMusicInfoOrderInMusicListCommandValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.UserId).NotNull();

        RuleFor(x => x.MusicListId).NotNull();

        RuleFor(x => x.TargetMusicInfoId).NotNull();

        RuleFor(x => x.PrevMusicInfoId);

        RuleFor(x => x.NextMusicInfoId);
    }
}
