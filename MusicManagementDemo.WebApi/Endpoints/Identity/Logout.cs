using System.Security.Claims;
using Mediator;
using MusicManagementDemo.Application.UseCase.Identity.Logout;
using MusicManagementDemo.WebApi.Utils;
using RustSharp;

namespace MusicManagementDemo.WebApi.Endpoints.Identity;

[RegisterTransient<IEndpoint>(Duplicate = DuplicateStrategy.Append)]
internal sealed class Logout : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "api/identity/logout",
                async (
                    ClaimsPrincipal claimsPrincipal,
                    IMediator mediator,
                    CancellationToken cancellationToken
                ) =>
                {
                    var userId = claimsPrincipal.GetUserId();
                    return userId switch
                    {
                        NoneOption<Guid> => Results.Unauthorized(),
                        SomeOption<Guid> someOption => Results.Ok(
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
