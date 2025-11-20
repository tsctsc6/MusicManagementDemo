using System.Security.Claims;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using MusicManagementDemo.Application.Responses;
using MusicManagementDemo.Application.UseCase.Identity.Logout;
using MusicManagementDemo.WebApi.Utils;
using RustSharp;

namespace MusicManagementDemo.WebApi.Endpoints.Identity;

[RegisterTransient<IEndpoint>(Duplicate = DuplicateStrategy.Append, Tags = InjectioTags.Endpoint)]
internal sealed class Logout : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "api/identity/logout",
                async Task<Results<Ok<ApiResult<LogoutCommandResponse>>, UnauthorizedHttpResult>> (
                    ClaimsPrincipal claimsPrincipal,
                    IMediator mediator,
                    CancellationToken cancellationToken
                ) =>
                {
                    var userId = claimsPrincipal.GetUserId();
                    return userId switch
                    {
                        NoneOption<Guid> => TypedResults.Unauthorized(),
                        SomeOption<Guid> someOption => TypedResults.Ok(
                            await mediator.Send(
                                new LogoutCommand(someOption.Value),
                                cancellationToken
                            )
                        ),
                        _ => throw new ArgumentOutOfRangeException(nameof(userId)),
                    };
                }
            )
            .RequireAuthorization();
    }
}
