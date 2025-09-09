using MediatR;
using Microsoft.EntityFrameworkCore;
using MusicManagementDemo.Application.Responses;
using MusicManagementDemo.Infrastructure.Database;
using MusicManagementDemo.SharedKernel;

namespace MusicManagementDemo.Application.UseCase.Management.ReadStorage;

internal sealed class ReadStorageQueryHandler(ManagementAppDbContext dbContext)
    : IRequestHandler<ReadStorageQuery, IServiceResult>
{
    public async Task<IServiceResult> Handle(
        ReadStorageQuery request,
        CancellationToken cancellationToken
    )
    {
        var storageToRead = await dbContext.Storage.SingleOrDefaultAsync(
            cancellationToken: cancellationToken
        );
        if (storageToRead is null)
        {
            return ServiceResult.Err(503, ["没有找到对应的存储"]);
        }
        return ServiceResult<object>.Ok(
            new
            {
                storageToRead.Id,
                storageToRead.Name,
                storageToRead.Path,
            }
        );
    }
}
