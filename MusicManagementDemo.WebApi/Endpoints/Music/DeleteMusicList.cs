using System.Security.Claims;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using MusicManagementDemo.Application.Responses;
using MusicManagementDemo.Application.UseCase.Music.DeleteMusicList;
using MusicManagementDemo.WebApi.Utils;
using RustSharp;

namespace MusicManagementDemo.WebApi.Endpoints.Music;

internal sealed record DeleteMusicListRequest(Guid MusicListId);

[RegisterTransient<IEndpoint>(Duplicate = DuplicateStrategy.Append, Tags = InjectioTags.Endpoint)]
internal sealed class DeleteMusicList : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "api/music/delete-music-list",
                async Task<
                    Results<Ok<ApiResult<DeleteMusicListCommandResponse>>, UnauthorizedHttpResult>
                > (
                    DeleteMusicListRequest request,
                    ClaimsPrincipal claimsPrincipal,
                    IMediator mediator,
                    CancellationToken cancellationToken
                ) =>
                {
                    var optionalUserId = claimsPrincipal.GetUserId();
                    return optionalUserId switch
                    {
                        NoneOption<Guid> => TypedResults.Unauthorized(),
                        SomeOption<Guid> userId => TypedResults.Ok(
                            await mediator.Send(
                                new DeleteMusicListCommand(userId.Value, request.MusicListId),
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
