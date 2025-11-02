using System.Security.Claims;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using MusicManagementDemo.Application.UseCase.Music.UpdateMusicList;
using MusicManagementDemo.WebApi.Utils;
using RustSharp;

namespace MusicManagementDemo.WebApi.Endpoints.Music;

internal sealed class UpdateMusicList : IEndpoint
{
    private sealed record Request(Guid MusicListId, string Name);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "api/music/update-music-list",
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
                                new UpdateMusicListCommand(
                                    userId.Value,
                                    request.MusicListId,
                                    request.Name
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
