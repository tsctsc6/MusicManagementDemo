using System.Security.Claims;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using MusicManagementDemo.Application.UseCase.Music.GetAllMusicInfoFromMusicList;
using MusicManagementDemo.WebApi.Utils;
using RustSharp;

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
                    var optionalUserId = claimsPrincipal.GetUserId();
                    return optionalUserId switch
                    {
                        NoneOption<Guid> => Results.Unauthorized(),
                        SomeOption<Guid> userId => Results.Ok(
                            await mediator.Send(
                                new GetAllMusicInfoFromMusicListQuery(
                                    UserId: userId.Value,
                                    MusicListId: musicListId,
                                    ReferenceId: referenceId,
                                    PageSize: pageSize ?? 10,
                                    Asc: asc ?? false
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
