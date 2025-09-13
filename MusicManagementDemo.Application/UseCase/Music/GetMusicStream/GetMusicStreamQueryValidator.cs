using FluentValidation;

namespace MusicManagementDemo.Application.UseCase.Music.GetMusicStream;

public class GetMusicStreamQueryValidator : AbstractValidator<GetMusicStreamQuery>
{
    public GetMusicStreamQueryValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(e => e.MusicInfoId).NotNull();
    }
}
