using MediatR;
using MusicManagementDemo.SharedKernel;

namespace MusicManagementDemo.Application.UseCase.Management.UpdateStorage;

internal sealed record UpdateStorageCommand(int Id, string Name, string Path)
    : IRequest<IServiceResult>;
