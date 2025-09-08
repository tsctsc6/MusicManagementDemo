using MediatR;
using Microsoft.AspNetCore.Authorization;
using MusicManagementDemo.Application.UseCase.Management.UpdateStorage;

namespace MusicManagementDemo.WebApi.Endpoints.Management;

internal sealed class UpdateStorage : IEndpoint
{
    private sealed record Request(int Id, string Name, string Path);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "api/management/update-storage",
                async (Request request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(
                        new UpdateStorageCommand(
                            Id: request.Id,
                            Name: request.Name,
                            Path: request.Path
                        ),
                        cancellationToken
                    );
                    return Results.Ok(result);
                }
            )
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" });
    }
}
