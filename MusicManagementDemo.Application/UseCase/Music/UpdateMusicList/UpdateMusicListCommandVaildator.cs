using FluentValidation;

namespace MusicManagementDemo.Application.UseCase.Music.UpdateMusicList;

internal sealed class UpdateMusicListCommandVaildator : AbstractValidator<UpdateMusicListCommand>
{
    public UpdateMusicListCommandVaildator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(e => e.UserId).NotNull();

        RuleFor(e => e.MusicListId).NotNull();

        RuleFor(e => e.Name).NotEmpty().MaximumLength(100);
    }
}
