using Mediator;
using Microsoft.AspNetCore.Authorization;
using MusicManagementDemo.Application.UseCase.Management.CreateStorage;

namespace MusicManagementDemo.WebApi.Endpoints.Management;

internal sealed record CreateStorageRequest(string Name, string Path);

[RegisterTransient<IEndpoint>(Duplicate = DuplicateStrategy.Append, Tags = InjectioTags.Endpoint)]
internal sealed class CreateStorage : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "api/management/create-storage",
                async (
                    CreateStorageRequest request,
                    IMediator mediator,
                    CancellationToken cancellationToken
                ) =>
                {
                    var result = await mediator.Send(
                        new CreateStorageCommand(Name: request.Name, Path: request.Path),
                        cancellationToken
                    );
                    return TypedResults.Ok(result);
                }
            )
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" });
    }
}
