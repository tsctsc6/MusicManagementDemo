using Mediator;
using MusicManagementDemo.Application.UseCase.Identity.Login;

namespace MusicManagementDemo.WebApi.Endpoints.Identity;

internal sealed record LoginRequest(string Email, string Password);

[RegisterTransient<IEndpoint>(Duplicate = DuplicateStrategy.Append, Tags = InjectioTags.Endpoint)]
internal sealed class Login : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
            "api/identity/login",
            async (LoginRequest request, IMediator mediator, CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(
                    new LoginCommand(Email: request.Email, Password: request.Password),
                    cancellationToken
                );
                return TypedResults.Ok(result);
            }
        );
    }
}
