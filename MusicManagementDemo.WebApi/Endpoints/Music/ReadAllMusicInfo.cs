using MediatR;
using Microsoft.AspNetCore.Authorization;
using MusicManagementDemo.Application.UseCase.Music.ReadAllMusicInfo;

namespace MusicManagementDemo.WebApi.Endpoints.Music;

public class ReadAllMusicInfo : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "api/music/read-all-music-info",
                async (
                    int? pageSize,
                    Guid? referenceId,
                    bool? asc,
                    IMediator mediator,
                    CancellationToken cancellationToken
                ) =>
                {
                    var result = await mediator.Send(
                        new ReadAllMusicInfoQuery(
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
