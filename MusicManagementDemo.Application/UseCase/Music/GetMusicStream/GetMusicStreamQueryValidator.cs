using FluentValidation;

namespace MusicManagementDemo.Application.UseCase.Music.GetMusicStream;

[RegisterScoped<IValidator<GetMusicStreamQuery>>(
    Duplicate = DuplicateStrategy.Append,
    Tags = InjectioTags.Validator
)]
public class GetMusicStreamQueryValidator : AbstractValidator<GetMusicStreamQuery>
{
    public GetMusicStreamQueryValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(e => e.MusicInfoId).NotNull();
    }
}
