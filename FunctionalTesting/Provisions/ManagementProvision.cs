using Mediator;
using MusicManagementDemo.Application.UseCase.Management.CreateJob;
using MusicManagementDemo.Application.UseCase.Management.CreateStorage;

namespace FunctionalTesting.Provisions;

public static class ManagementProvision
{
    public static async Task<int> CreateStorageAsync(
        IMediator mediator,
        CreateStorageCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.Data!.Id;
    }

    public static async Task<long> CreateJobAsync(
        IMediator mediator,
        CreateJobCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.Data!.JobId;
    }
}
