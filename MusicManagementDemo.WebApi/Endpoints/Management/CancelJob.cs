using MediatR;
using Microsoft.AspNetCore.Authorization;
using MusicManagementDemo.Application.UseCase.Management.CancelJob;

namespace MusicManagementDemo.WebApi.Endpoints.Management;

internal sealed class CancelJob : IEndpoint
{
    private sealed record Request(long JobId);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "api/management/Cancel-job",
                async (Request request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(
                        new CancelJobCommand(request.JobId),
                        cancellationToken
                    );
                    return Results.Ok(result);
                }
            )
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" });
    }
}
