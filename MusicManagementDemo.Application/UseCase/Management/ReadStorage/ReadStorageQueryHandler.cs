using MediatR;
using MusicManagementDemo.Infrastructure.Database;
using MusicManagementDemo.SharedKernel;

namespace MusicManagementDemo.Application.UseCase.Management.ReadStorage;

internal sealed class ReadStorageQueryHandler(ManagementAppDbContext dbContext)
    : IRequestHandler<ReadStorageQuery, IServiceResult>
{
    public Task<IServiceResult> Handle(
        ReadStorageQuery request,
        CancellationToken cancellationToken
    )
    {
        throw new NotImplementedException();
    }
}
