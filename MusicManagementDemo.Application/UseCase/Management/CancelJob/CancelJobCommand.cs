using Mediator;

namespace MusicManagementDemo.Application.UseCase.Management.CancelJob;

public sealed record CancelJobCommand(long JobId) : IRequest<IServiceResult>;
