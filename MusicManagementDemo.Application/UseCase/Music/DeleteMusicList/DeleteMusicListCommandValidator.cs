using FluentValidation;

namespace MusicManagementDemo.Application.UseCase.Music.DeleteMusicList;

[RegisterScoped<IValidator<DeleteMusicListCommand>>(
    Duplicate = DuplicateStrategy.Append,
    Tags = InjectioTags.Validator
)]
internal sealed class DeleteMusicListCommandValidator : AbstractValidator<DeleteMusicListCommand>
{
    public DeleteMusicListCommandValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(e => e.UserId).NotNull();

        RuleFor(e => e.MusicListId).NotNull();
    }
}
