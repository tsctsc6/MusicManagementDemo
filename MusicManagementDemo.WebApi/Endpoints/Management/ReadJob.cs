using Mediator;
using Microsoft.AspNetCore.Authorization;
using MusicManagementDemo.Application.UseCase.Management.ReadJob;

namespace MusicManagementDemo.WebApi.Endpoints.Management;

internal sealed class ReadJob : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "api/management/read-job",
                async (long id, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new ReadJobQuery(Id: id), cancellationToken);
                    return Results.Ok(result);
                }
            )
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" });
    }
}
