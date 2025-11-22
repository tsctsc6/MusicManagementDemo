using System.Security.Claims;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using MusicManagementDemo.Application.Responses;
using MusicManagementDemo.Application.UseCase.Music.ChangeMusicInfoOrderInMusicList;
using MusicManagementDemo.WebApi.Utils;
using RustSharp;

namespace MusicManagementDemo.WebApi.Endpoints.Music;

internal sealed record ChangeMusicInfoOrderInMusicListRequest(
    Guid MusicListId,
    Guid TargetMusicInfoId,
    Guid? PrevMusicInfoId,
    Guid? NextMusicInfoId
);

[RegisterTransient<IEndpoint>(Duplicate = DuplicateStrategy.Append, Tags = InjectioTags.Endpoint)]
internal sealed class ChangeMusicInfoOrderInMusicList : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "api/music/change-music-info-order-in-music-list",
                async Task<
                    Results<
                        Ok<ApiResult<ChangeMusicInfoOrderInMusicListCommandResponse>>,
                        UnauthorizedHttpResult
                    >
                > (
                    ChangeMusicInfoOrderInMusicListRequest request,
                    ClaimsPrincipal claimsPrincipal,
                    IMediator mediator,
                    CancellationToken cancellationToken
                ) =>
                {
                    var optionalUserId = claimsPrincipal.GetUserId();
                    return optionalUserId switch
                    {
                        NoneOption<Guid> => TypedResults.Unauthorized(),
                        SomeOption<Guid> userId => TypedResults.Ok(
                            await mediator.Send(
                                new ChangeMusicInfoOrderInMusicListCommand(
                                    userId.Value,
                                    request.MusicListId,
                                    request.TargetMusicInfoId,
                                    request.PrevMusicInfoId,
                                    request.NextMusicInfoId
                                ),
                                cancellationToken
                            )
                        ),
                        _ => throw new ArgumentOutOfRangeException(nameof(optionalUserId)),
                    };
                }
            )
            .RequireAuthorization(new AuthorizeAttribute())
            .Produces<UnauthorizedHttpResult>(StatusCodes.Status401Unauthorized)
            .WithName(nameof(ChangeMusicInfoOrderInMusicList));
    }
}
