using Mediator;
using Microsoft.AspNetCore.Authorization;
using MusicManagementDemo.Application.UseCase.Management.UpdateStorage;

namespace MusicManagementDemo.WebApi.Endpoints.Management;

internal sealed record UpdateStorageRequest(int Id, string Name, string Path);

[RegisterTransient<IEndpoint>(Duplicate = DuplicateStrategy.Append, Tags = InjectioTags.Endpoint)]
internal sealed class UpdateStorage : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "api/management/update-storage",
                async (
                    UpdateStorageRequest request,
                    IMediator mediator,
                    CancellationToken cancellationToken
                ) =>
                {
                    var result = await mediator.Send(
                        new UpdateStorageCommand(
                            Id: request.Id,
                            Name: request.Name,
                            Path: request.Path
                        ),
                        cancellationToken
                    );
                    return TypedResults.Ok(result);
                }
            )
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" })
            .WithName(nameof(UpdateStorage));
    }
}
