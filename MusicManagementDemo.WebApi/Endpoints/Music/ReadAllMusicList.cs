using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using MusicManagementDemo.Application.UseCase.Music.ReadAllMusicList;

namespace MusicManagementDemo.WebApi.Endpoints.Music;

internal sealed class ReadAllMusicList : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "api/music/read-all-music-list",
                async (
                    int? pageSize,
                    Guid? referenceId,
                    bool? asc,
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
                        new ReadAllMusicListQuery(
                            UserId: userId,
                            ReferenceId: referenceId,
                            PageSize: pageSize ?? 10,
                            Asc: asc ?? false
                        ),
                        cancellationToken
                    );
                    return Results.Ok(result);
                }
            )
            .RequireAuthorization(new AuthorizeAttribute());
    }
}
