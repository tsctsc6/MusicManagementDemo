using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using MusicManagementDemo.Application.UseCase.Music.AddMusicInfoToMusicList;

namespace MusicManagementDemo.WebApi.Endpoints.Music;

internal sealed class AddMusicInfoToMusicList : IEndpoint
{
    private sealed record Request(Guid MusicListId, Guid MusicInfoId);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "api/music/add-music-info-to-music-list",
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
                        new AddMusicInfoToMusicListCommand(
                            userId,
                            request.MusicListId,
                            request.MusicInfoId
                        ),
                        cancellationToken
                    );
                    return Results.Ok(result);
                }
            )
            .RequireAuthorization(new AuthorizeAttribute());
    }
}
