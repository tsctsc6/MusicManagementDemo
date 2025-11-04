using FluentValidation;

namespace MusicManagementDemo.Application.UseCase.Music.ReadAllMusicList;

[RegisterScoped<IValidator<ReadAllMusicListQuery>>(
    Duplicate = DuplicateStrategy.Append,
    Tags = "Validator"
)]
internal sealed class ReadAllMusicListQueryValidator : AbstractValidator<ReadAllMusicListQuery>
{
    public ReadAllMusicListQueryValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.UserId).NotNull();

        RuleFor(x => x.ReferenceId);

        RuleFor(x => x.PageSize).NotNull().GreaterThan(0).LessThan(30);

        RuleFor(x => x.Asc).NotNull();
    }
}
