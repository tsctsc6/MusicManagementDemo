﻿using MediatR;
using Microsoft.AspNetCore.Authorization;
using MusicManagementDemo.Application.UseCase.Management.ReadStorage;

namespace MusicManagementDemo.WebApi.Endpoints.Management;

internal sealed class ReadStorage : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "api/management/read-storage",
                async (int id, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(
                        new ReadStorageQuery(Id: id),
                        cancellationToken
                    );
                    return Results.Ok(result);
                }
            )
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" });
    }
}
