using FluentValidation;

namespace MusicManagementDemo.Application.UseCase.Music.RemoveMusicInfoFromMusicList;

[RegisterScoped<IValidator<RemoveMusicInfoFromMusicListCommand>>(
    Duplicate = DuplicateStrategy.Append
)]
internal sealed class RemoveMusicInfoFromMusicListCommandValidator
    : AbstractValidator<RemoveMusicInfoFromMusicListCommand>
{
    public RemoveMusicInfoFromMusicListCommandValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(e => e.UserId).NotNull();

        RuleFor(e => e.MusicListId).NotNull();

        RuleFor(e => e.MusicListId).NotNull();
    }
}
