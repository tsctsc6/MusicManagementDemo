using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using MusicManagementDemo.Application.UseCase.Music.CreateMusicList;
using MusicManagementDemo.WebApi.Utils;
using RustSharp;

namespace MusicManagementDemo.WebApi.Endpoints.Music;

internal sealed class CreateMusicList : IEndpoint
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
                    var optionalUserId = claimsPrincipal.GetUserId();
                    return optionalUserId switch
                    {
                        NoneOption<Guid> => Results.Unauthorized(),
                        SomeOption<Guid> userId => Results.Ok(
                            await mediator.Send(
                                new CreateMusicListCommand(userId.Value, request.Name),
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
