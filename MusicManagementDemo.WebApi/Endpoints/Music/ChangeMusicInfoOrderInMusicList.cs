using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using MusicManagementDemo.Application.UseCase.Music.ChangeMusicInfoOrderInMusicList;

namespace MusicManagementDemo.WebApi.Endpoints.Music;

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
                    var userId =
                        claimsPrincipal
                            .Claims.SingleOrDefault(e => e.Type == JwtRegisteredClaimNames.Sub)
                            ?.Value
                        ?? string.Empty;
                    var result = await mediator.Send(
                        new ChangeMusicInfoOrderInMusicListCommand(
                            userId,
                            request.MusicListId,
                            request.TargetMusicInfoId,
                            request.PrevMusicInfoId,
                            request.NextMusicInfoId
                        ),
                        cancellationToken
                    );
                    return Results.Ok(result);
                }
            )
            .RequireAuthorization(new AuthorizeAttribute());
    }
}
