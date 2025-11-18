using System.Text.Json.Nodes;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using MusicManagementDemo.Application.UseCase.Management.CreateJob;
using MusicManagementDemo.Domain.Entity.Management;

namespace MusicManagementDemo.WebApi.Endpoints.Management;

[RegisterTransient<IEndpoint>(Duplicate = DuplicateStrategy.Append, Tags = "Endpoint")]
internal sealed class CreateJob : IEndpoint
{
    private sealed record Request(JobType Type, string Description, JsonNode JobArgs);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "api/management/create-job",
                async (Request request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(
                        new CreateJobCommand(request.Type, request.Description, request.JobArgs),
                        cancellationToken
                    );
                    return TypedResults.Ok(result);
                }
            )
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" });
    }
}
