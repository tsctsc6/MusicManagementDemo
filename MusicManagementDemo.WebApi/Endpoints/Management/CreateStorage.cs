using Mediator;
using Microsoft.AspNetCore.Authorization;
using MusicManagementDemo.Application.UseCase.Management.CreateStorage;

namespace MusicManagementDemo.WebApi.Endpoints.Management;

[RegisterTransient<IEndpoint>(Duplicate = DuplicateStrategy.Append)]
internal sealed class CreateStorage : IEndpoint
{
    private sealed record Request(string Name, string Path);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "api/management/create-storage",
                async (Request request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(
                        new CreateStorageCommand(Name: request.Name, Path: request.Path),
                        cancellationToken
                    );
                    return Results.Ok(result);
                }
            )
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" });
    }
}
