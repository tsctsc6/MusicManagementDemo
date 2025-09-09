using MediatR;
using MusicManagementDemo.SharedKernel;

namespace MusicManagementDemo.Application.UseCase.Management.ReadStorage;

public sealed record ReadStorageQuery(int Id) : IRequest<IServiceResult>;
