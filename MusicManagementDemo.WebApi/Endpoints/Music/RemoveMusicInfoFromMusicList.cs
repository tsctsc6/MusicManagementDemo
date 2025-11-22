using System.Security.Claims;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using MusicManagementDemo.Application.Responses;
using MusicManagementDemo.Application.UseCase.Music.RemoveMusicInfoFromMusicList;
using MusicManagementDemo.WebApi.Utils;
using RustSharp;

namespace MusicManagementDemo.WebApi.Endpoints.Music;

internal sealed record RemoveMusicInfoFromMusicListRequest(Guid MusicListId, Guid MusicInfoId);

[RegisterTransient<IEndpoint>(Duplicate = DuplicateStrategy.Append, Tags = InjectioTags.Endpoint)]
internal sealed class RemoveMusicInfoFromMusicList : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "api/music/remove-music-info-from-music-list",
                async Task<
                    Results<
                        Ok<ApiResult<RemoveMusicInfoFromMusicListCommandResponse>>,
                        UnauthorizedHttpResult
                    >
                > (
                    RemoveMusicInfoFromMusicListRequest removeMusicInfoFromMusicListRequest,
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
                                new RemoveMusicInfoFromMusicListCommand(
                                    userId.Value,
                                    removeMusicInfoFromMusicListRequest.MusicListId,
                                    removeMusicInfoFromMusicListRequest.MusicInfoId
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
            .WithName(nameof(RemoveMusicInfoFromMusicList));
    }
}
