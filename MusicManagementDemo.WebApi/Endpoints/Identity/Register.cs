
namespace MusicManagementDemo.WebApi.Endpoints.Identity;

internal sealed class Register : IEndpoint
{
    public sealed record Request(string Email, string UserName, string Password);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/identity/register", async (
            Request request,
            CancellationToken cancellationToken) =>
        {
            return Results.Ok();
        });
    }
}
