using Mediator;

namespace MusicManagementDemo.Application.UseCase.Management.CreateStorage;

public sealed record CreateStorageCommand(string Name, string Path) : IRequest<IServiceResult>;
