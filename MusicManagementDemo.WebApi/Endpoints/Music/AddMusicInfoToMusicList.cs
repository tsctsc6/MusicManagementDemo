using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using MusicManagementDemo.Application.UseCase.Music.AddMusicInfoToMusicList;
using MusicManagementDemo.WebApi.Utils;
using RustSharp;

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
                    var optionalUserId = claimsPrincipal.GetUserId();
                    return optionalUserId switch
                    {
                        NoneOption<Guid> => Results.Unauthorized(),
                        SomeOption<Guid> userId => Results.Ok(
                            await mediator.Send(
                                new AddMusicInfoToMusicListCommand(
                                    userId.Value,
                                    request.MusicListId,
                                    request.MusicInfoId
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
