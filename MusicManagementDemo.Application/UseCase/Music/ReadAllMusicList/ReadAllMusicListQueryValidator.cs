using FluentValidation;

namespace MusicManagementDemo.Application.UseCase.Music.ReadAllMusicList;

internal sealed class ReadAllMusicListQueryValidator : AbstractValidator<ReadAllMusicListQuery>
{
    public ReadAllMusicListQueryValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.UserId).NotEmpty().MaximumLength(36);

        RuleFor(x => x.ReferenceId).NotNull();

        RuleFor(x => x.PageSize).NotNull().GreaterThan(0).LessThan(30);
    }
}
