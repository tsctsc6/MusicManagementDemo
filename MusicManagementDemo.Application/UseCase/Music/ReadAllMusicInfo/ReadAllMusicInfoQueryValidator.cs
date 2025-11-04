using FluentValidation;

namespace MusicManagementDemo.Application.UseCase.Music.ReadAllMusicInfo;

[RegisterScoped<IValidator<ReadAllMusicInfoQuery>>(
    Duplicate = DuplicateStrategy.Append,
    Tags = "Validator"
)]
internal sealed class ReadAllMusicInfoQueryValidator : AbstractValidator<ReadAllMusicInfoQuery>
{
    public ReadAllMusicInfoQueryValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.ReferenceId);

        RuleFor(x => x.PageSize).NotNull().GreaterThan(0).LessThan(30);

        RuleFor(x => x.Asc).NotNull();

        RuleFor(x => x.SearchTerm).NotNull().MaximumLength(200);
    }
}
