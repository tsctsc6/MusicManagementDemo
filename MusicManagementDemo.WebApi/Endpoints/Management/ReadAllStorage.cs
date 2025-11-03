using Mediator;
using Microsoft.AspNetCore.Authorization;
using MusicManagementDemo.Application.UseCase.Management.ReadAllStorage;

namespace MusicManagementDemo.WebApi.Endpoints.Management;

[RegisterTransient<IEndpoint>(Duplicate = DuplicateStrategy.Append)]
internal sealed class ReadAllStorage : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "api/management/read-all-storage",
                async (
                    int? referenceId,
                    int? pageSize,
                    bool? asc,
                    IMediator mediator,
                    CancellationToken cancellationToken
                ) =>
                {
                    var result = await mediator.Send(
                        new ReadAllStorageQuery(
                            ReferenceId: referenceId,
                            PageSize: pageSize ?? 10,
                            Asc: asc ?? false
                        ),
                        cancellationToken
                    );
                    return Results.Ok(result);
                }
            )
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" });
    }
}
