using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MediatR;
using MusicManagementDemo.Application.UseCase.Identity.Logout;

namespace MusicManagementDemo.WebApi.Endpoints.Identity;

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
                    var userId =
                        claimsPrincipal
                            .Claims.SingleOrDefault(e => e.Type == JwtRegisteredClaimNames.Sub)
                            ?.Value
                        ?? string.Empty;
                    var result = await mediator.Send(new LogoutCommand(userId), cancellationToken);
                    return Results.Ok(result);
                }
            )
            .RequireAuthorization();
    }
}
