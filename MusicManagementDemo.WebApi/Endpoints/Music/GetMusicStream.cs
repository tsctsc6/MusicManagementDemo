using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using MusicManagementDemo.Application.UseCase.Music.GetMusicStream;
using RustSharp;

namespace MusicManagementDemo.WebApi.Endpoints.Music;

[RegisterTransient<IEndpoint>(Duplicate = DuplicateStrategy.Append, Tags = "Endpoint")]
internal sealed class GetMusicStream : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "api/music/get-music-stream/{musicStreamId:guid}",
                async Task<Results<FileStreamHttpResult, NotFound>> (
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
                        SomeOption<Stream> stream => TypedResults.File(
                            fileStream: stream.Value,
                            contentType: "video/x-flac",
                            enableRangeProcessing: true
                        ),
                        _ => TypedResults.NotFound(),
                    };
                }
            )
            .RequireAuthorization(new AuthorizeAttribute());
    }
}
