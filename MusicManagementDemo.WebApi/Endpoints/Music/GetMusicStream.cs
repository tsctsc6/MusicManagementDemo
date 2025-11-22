using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.OpenApi;
using MusicManagementDemo.Application.UseCase.Music.GetMusicStream;
using RustSharp;

namespace MusicManagementDemo.WebApi.Endpoints.Music;

[RegisterTransient<IEndpoint>(Duplicate = DuplicateStrategy.Append, Tags = InjectioTags.Endpoint)]
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
            .RequireAuthorization(new AuthorizeAttribute())
            .AddOpenApiOperationTransformer(
                (operation, context, ct) =>
                {
                    operation.Responses!["200"] = new OpenApiResponse
                    {
                        Description = "OK",
                        Content = new Dictionary<string, OpenApiMediaType>
                        {
                            ["video/x-flac"] = new()
                            {
                                Schema = new OpenApiSchema
                                {
                                    Type = JsonSchemaType.String,
                                    Format = "binary",
                                },
                            },
                        },
                    };
                    operation.Responses!["206"] = new OpenApiResponse
                    {
                        Description = "Partial Content",
                        Content = new Dictionary<string, OpenApiMediaType>
                        {
                            ["video/x-flac"] = new()
                            {
                                Schema = new OpenApiSchema
                                {
                                    Type = JsonSchemaType.String,
                                    Format = "binary",
                                },
                            },
                        },
                    };
                    return Task.CompletedTask;
                }
            )
            .WithName(nameof(GetMusicStream));
    }
}
