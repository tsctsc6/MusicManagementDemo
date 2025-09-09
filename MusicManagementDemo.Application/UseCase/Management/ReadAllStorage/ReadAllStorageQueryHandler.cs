using MediatR;
using Microsoft.EntityFrameworkCore;
using MusicManagementDemo.Application.Responses;
using MusicManagementDemo.Domain.Entity.Management;
using MusicManagementDemo.Infrastructure.Database;
using MusicManagementDemo.SharedKernel;

namespace MusicManagementDemo.Application.UseCase.Management.ReadAllStorage;

internal sealed class ReadAllStorageQueryHandler(ManagementAppDbContext dbContext)
    : IRequestHandler<ReadAllStorageQuery, IServiceResult>
{
    public async Task<IServiceResult> Handle(
        ReadAllStorageQuery request,
        CancellationToken cancellationToken
    )
    {
        var storagesToRead = await dbContext
            .Storage.AsNoTracking()
            .Skip(request.Page * request.PageSize)
            .Take(request.PageSize)
            .ToArrayAsync(cancellationToken: cancellationToken);

        return ServiceResult<Storage[]>.Ok(storagesToRead);
    }
}
