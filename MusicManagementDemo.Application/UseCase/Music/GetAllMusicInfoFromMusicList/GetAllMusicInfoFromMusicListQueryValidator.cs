using FluentValidation;

namespace MusicManagementDemo.Application.UseCase.Music.GetAllMusicInfoFromMusicList;

[RegisterScoped<IValidator<GetAllMusicInfoFromMusicListQuery>>(
    Duplicate = DuplicateStrategy.Append,
    Tags = InjectioTags.Validator
)]
internal sealed class GetAllMusicInfoFromMusicListQueryValidator
    : AbstractValidator<GetAllMusicInfoFromMusicListQuery>
{
    public GetAllMusicInfoFromMusicListQueryValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(e => e.UserId).NotNull();

        RuleFor(e => e.MusicListId).NotNull();

        RuleFor(x => x.ReferenceId);

        RuleFor(x => x.PageSize).NotNull().GreaterThan(0).LessThan(30);

        RuleFor(x => x.Asc).NotNull();
    }
}
