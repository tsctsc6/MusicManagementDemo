using MediatR;
using MusicManagementDemo.Application.UseCase.Identity.Register;

namespace FunctionalTesting.Provisions;

public static class IdentityProvision
{
    public static async Task<Guid> RegisterAsync(
        IMediator mediator,
        RegisterCommand command,
        CancellationToken cancellationToken
    )
    {
        var result = await mediator.Send(command, cancellationToken);
        return Guid.Parse(result.Data!.GetPropertyValue("Id")!.ToString()!);
    }
}
