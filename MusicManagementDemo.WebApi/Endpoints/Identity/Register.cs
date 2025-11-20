using Mediator;
using MusicManagementDemo.Application.UseCase.Identity.Register;

namespace MusicManagementDemo.WebApi.Endpoints.Identity;

internal sealed record RegisterRequest(string Email, string UserName, string Password);

[RegisterTransient<IEndpoint>(Duplicate = DuplicateStrategy.Append, Tags = InjectioTags.Endpoint)]
internal sealed class Register : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "api/identity/register",
                async (
                    RegisterRequest request,
                    IMediator mediator,
                    CancellationToken cancellationToken
                ) =>
                {
                    var result = await mediator.Send(
                        new RegisterCommand(
                            Email: request.Email,
                            UserName: request.UserName,
                            Password: request.Password
                        ),
                        cancellationToken
                    );
                    return TypedResults.Ok(result);
                }
            )
            .WithName(nameof(Register));
    }
}
