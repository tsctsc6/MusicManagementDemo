using MediatR;
using MusicManagementDemo.Application.UseCase.Identity.Login;

namespace MusicManagementDemo.WebApi.Endpoints.Identity;

public class Login : IEndpoint
{
    public sealed record Request(string Email, string UserName, string Password);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
            "api/identity/login",
            async (Request request, IMediator mediator, CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(
                    new LoginCommand(Email: request.Email, Password: request.Password),
                    cancellationToken
                );
                return Results.Ok(result);
            }
        );
    }
}
