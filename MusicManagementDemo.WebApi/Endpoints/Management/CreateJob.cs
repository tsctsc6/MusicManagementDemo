using System.Text.Json.Nodes;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using MusicManagementDemo.Application.UseCase.Management.CreateJob;
using MusicManagementDemo.Domain.Entity.Management;

namespace MusicManagementDemo.WebApi.Endpoints.Management;

internal sealed record CreateJobRequest(JobType Type, string Description, JsonNode JobArgs);

[RegisterTransient<IEndpoint>(Duplicate = DuplicateStrategy.Append, Tags = InjectioTags.Endpoint)]
internal sealed class CreateJob : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "api/management/create-job",
                async (
                    CreateJobRequest request,
                    IMediator mediator,
                    CancellationToken cancellationToken
                ) =>
                {
                    var result = await mediator.Send(
                        new CreateJobCommand(request.Type, request.Description, request.JobArgs),
                        cancellationToken
                    );
                    return TypedResults.Ok(result);
                }
            )
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" })
            .WithName(nameof(CreateJob));
    }
}
