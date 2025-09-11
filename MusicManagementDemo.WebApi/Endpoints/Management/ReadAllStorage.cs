using MediatR;
using Microsoft.AspNetCore.Authorization;
using MusicManagementDemo.Application.UseCase.Management.ReadAllStorage;

namespace MusicManagementDemo.WebApi.Endpoints.Management;

internal sealed class ReadAllStorage : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "api/management/read-all-storage",
                async (
                    int page,
                    int pageSize,
                    IMediator mediator,
                    CancellationToken cancellationToken
                ) =>
                {
                    var result = await mediator.Send(
                        new ReadAllStorageQuery(Page: page, PageSize: pageSize),
                        cancellationToken
                    );
                    return Results.Ok(result);
                }
            )
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" });
    }
}
