using Mediator;
using MusicManagementDemo.Application.UseCase.Identity.Register;

namespace MusicManagementDemo.WebApi.Endpoints.Identity;

[RegisterTransient<IEndpoint>(Duplicate = DuplicateStrategy.Append)]
internal sealed class Register : IEndpoint
{
    private sealed record Request(string Email, string UserName, string Password);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
            "api/identity/register",
            async (Request request, IMediator mediator, CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(
                    new RegisterCommand(
                        Email: request.Email,
                        UserName: request.UserName,
                        Password: request.Password
                    ),
                    cancellationToken
                );
                return Results.Ok(result);
            }
        );
    }
}
