using Mediator;
using Microsoft.AspNetCore.Authorization;
using MusicManagementDemo.Application.UseCase.Music.GetMusicStream;
using RustSharp;

namespace MusicManagementDemo.WebApi.Endpoints.Music;

internal sealed class GetMusicStream : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "api/music/get-music-stream/{musicStreamId:guid}",
                async (
                    Guid musicStreamId,
                    IMediator mediator,
                    CancellationToken cancellationToken
                ) =>
                {
                    var optionalStream = await mediator.Send(
                        new GetMusicStreamQuery(musicStreamId),
                        cancellationToken
                    );
                    return optionalStream switch
                    {
                        SomeOption<Stream> stream => Results.File(
                            fileStream: stream.Value,
                            contentType: "video/x-flac",
                            enableRangeProcessing: true
                        ),
                        _ => Results.NotFound(),
                    };
                }
            )
            .RequireAuthorization(new AuthorizeAttribute());
    }
}
