using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using MusicManagementDemo.Application.UseCase.Music.GetAllMusicInfoFromMusicList;

namespace MusicManagementDemo.WebApi.Endpoints.Music;

internal sealed class GetAllMusicInfoFromMusicList : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "api/music/get-all-music-info-from-music-list",
                async (
                    Guid musicListId,
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
                        new GetAllMusicInfoFromMusicListQuery(
                            UserId: userId,
                            MusicListId: musicListId,
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
