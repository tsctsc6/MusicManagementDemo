using Mediator;
using Microsoft.AspNetCore.Authorization;
using MusicManagementDemo.Application.UseCase.Management.DeleteStorage;

namespace MusicManagementDemo.WebApi.Endpoints.Management;

[RegisterTransient<IEndpoint>(Duplicate = DuplicateStrategy.Append, Tags = InjectioTags.Endpoint)]
internal sealed class DeleteStorage : IEndpoint
{
    private sealed record Request(int Id);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "api/management/delete-storage",
                async (Request request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(
                        new DeleteStorageCommand(Id: request.Id),
                        cancellationToken
                    );
                    return TypedResults.Ok(result);
                }
            )
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" });
    }
}
