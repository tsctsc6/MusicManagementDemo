using Mediator;
using Microsoft.AspNetCore.Authorization;
using MusicManagementDemo.Application.UseCase.Music.ReadAllMusicInfo;

namespace MusicManagementDemo.WebApi.Endpoints.Music;

[RegisterTransient<IEndpoint>(Duplicate = DuplicateStrategy.Append)]
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
                    string? searchTerm,
                    IMediator mediator,
                    CancellationToken cancellationToken
                ) =>
                {
                    var result = await mediator.Send(
                        new ReadAllMusicInfoQuery(
                            ReferenceId: referenceId,
                            PageSize: pageSize ?? 10,
                            Asc: asc ?? false,
                            SearchTerm: searchTerm ?? string.Empty
                        ),
                        cancellationToken
                    );
                    return Results.Ok(result);
                }
            )
            .RequireAuthorization(new AuthorizeAttribute());
    }
}
