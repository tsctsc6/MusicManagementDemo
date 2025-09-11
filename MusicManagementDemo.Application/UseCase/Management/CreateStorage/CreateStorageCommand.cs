using MediatR;
using MusicManagementDemo.Abstractions;

namespace MusicManagementDemo.Application.UseCase.Management.CreateStorage;

public sealed record CreateStorageCommand(string Name, string Path) : IRequest<IServiceResult>;
