using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using MusicManagementDemo.Application.UseCase.Music.CreateMusicList;

namespace MusicManagementDemo.WebApi.Endpoints.Music;

public class CreateMusicList : IEndpoint
{
    private sealed record Request(string Name);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "api/music/create-music-list",
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
                        new CreateMusicListCommand(userId, request.Name),
                        cancellationToken
                    );
                    return Results.Ok(result);
                }
            )
            .RequireAuthorization(new AuthorizeAttribute());
    }
}
