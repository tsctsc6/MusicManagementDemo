using MediatR;
using MusicManagementDemo.Abstractions;

namespace MusicManagementDemo.Application.UseCase.Management.DeleteStorage;

public sealed record DeleteStorageCommand(int Id) : IRequest<IServiceResult>;
