using Mediator;
using MusicManagementDemo.Abstractions;

namespace MusicManagementDemo.Application.UseCase.Management.UpdateStorage;

public sealed record UpdateStorageCommand(int Id, string Name, string Path)
    : IRequest<IServiceResult>;
