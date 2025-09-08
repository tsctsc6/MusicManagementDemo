using MediatR;
using MusicManagementDemo.SharedKernel;

namespace MusicManagementDemo.Application.UseCase.Management.DeleteStorage;

public sealed record DeleteStorageCommand(int Id) : IRequest<IServiceResult>;
