﻿using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using MusicManagementDemo.Application.UseCase.Music.ReadAllMusicList;
using MusicManagementDemo.WebApi.Utils;
using RustSharp;

namespace MusicManagementDemo.WebApi.Endpoints.Music;

internal sealed class ReadAllMusicList : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "api/music/read-all-music-list",
                async (
                    int? pageSize,
                    Guid? referenceId,
                    bool? asc,
                    ClaimsPrincipal claimsPrincipal,
                    IMediator mediator,
                    CancellationToken cancellationToken
                ) =>
                {
                    var optionalUserId = claimsPrincipal.GetUserId();
                    return optionalUserId switch
                    {
                        NoneOption<Guid> => Results.Unauthorized(),
                        SomeOption<Guid> userId => Results.Ok(
                            await mediator.Send(
                                new ReadAllMusicListQuery(
                                    UserId: userId.Value,
                                    ReferenceId: referenceId,
                                    PageSize: pageSize ?? 10,
                                    Asc: asc ?? false
                                ),
                                cancellationToken
                            )
                        ),
                        _ => throw new ArgumentOutOfRangeException(nameof(optionalUserId)),
                    };
                }
            )
            .RequireAuthorization(new AuthorizeAttribute());
    }
}
