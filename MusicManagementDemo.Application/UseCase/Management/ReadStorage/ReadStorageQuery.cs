using MediatR;
using MusicManagementDemo.SharedKernel;

namespace MusicManagementDemo.Application.UseCase.Management.ReadStorage;

public record ReadStorageQuery(int Id) : IRequest<IServiceResult>;
