using Mediator;
using MusicManagementDemo.Application.UseCase.Identity.Register;

namespace FunctionalTesting.Provisions;

public static class IdentityProvision
{
    public static async Task<Guid> RegisterAsync(
        IMediator mediator,
        RegisterCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.Data!.Id;
    }
}
