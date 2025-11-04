using System.Security.Claims;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using MusicManagementDemo.Application.UseCase.Music.ChangeMusicInfoOrderInMusicList;
using MusicManagementDemo.WebApi.Utils;
using RustSharp;

namespace MusicManagementDemo.WebApi.Endpoints.Music;

[RegisterTransient<IEndpoint>(Duplicate = DuplicateStrategy.Append, Tags = "Endpoint")]
internal sealed class ChangeMusicInfoOrderInMusicList : IEndpoint
{
    private sealed record Request(
        Guid MusicListId,
        Guid TargetMusicInfoId,
        Guid? PrevMusicInfoId,
        Guid? NextMusicInfoId
    );

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "api/music/change-music-info-order-in-music-list",
                async (
                    Request request,
                    ClaimsPrincipal claimsPrincipal,
                    IMediator mediator,
                    CancellationToken cancellationToken
                ) =>
                {
                    var optionalUserId = claimsPrincipal.GetUserId();
                    return optionalUserId switch
                    {
                        NoneOption<Guid> => Results.Unauthorized(),
                        SomeOption<Guid> userId => Results.Ok(
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
            .RequireAuthorization(new AuthorizeAttribute());
    }
}
