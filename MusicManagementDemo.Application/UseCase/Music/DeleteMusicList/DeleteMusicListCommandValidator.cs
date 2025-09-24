using FluentValidation;

namespace MusicManagementDemo.Application.UseCase.Music.DeleteMusicList;

internal sealed class DeleteMusicListCommandValidator : AbstractValidator<DeleteMusicListCommand>
{
    public DeleteMusicListCommandValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(e => e.UserId).NotNull();

        RuleFor(e => e.MusicListId).NotNull();
    }
}
