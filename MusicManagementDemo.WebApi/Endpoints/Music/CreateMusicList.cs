using MediatR;
using Microsoft.AspNetCore.Authorization;
using MusicManagementDemo.Application.UseCase.Music.CreateMusicList;

namespace MusicManagementDemo.WebApi.Endpoints.Music;

public class CreateMusicList : IEndpoint
{
    private sealed record Request(string UserId, string Name);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "api/management/create-music-list",
                async (Request request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(
                        new CreateMusicListCommand(request.UserId, request.Name),
                        cancellationToken
                    );
                    return Results.Ok(result);
                }
            )
            .RequireAuthorization(new AuthorizeAttribute());
    }
}
