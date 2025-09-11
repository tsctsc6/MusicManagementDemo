using MediatR;
using Microsoft.EntityFrameworkCore;
using MusicManagementDemo.Abstractions;
using MusicManagementDemo.Abstractions.IDbContext;
using MusicManagementDemo.Application.Responses;

namespace MusicManagementDemo.Application.UseCase.Management.ReadStorage;

internal sealed class ReadStorageQueryHandler(IManagementAppDbContext dbContext)
    : IRequestHandler<ReadStorageQuery, IServiceResult>
{
    public async Task<IServiceResult> Handle(
        ReadStorageQuery request,
        CancellationToken cancellationToken
    )
    {
        var storageToRead = await dbContext
            .Storage.AsNoTracking()
            .SingleOrDefaultAsync(cancellationToken: cancellationToken);
        if (storageToRead is null)
        {
            return ServiceResult.Err(503, ["没有找到对应的存储"]);
        }
        return ServiceResult.Ok(
            new
            {
                storageToRead.Id,
                storageToRead.Name,
                storageToRead.Path,
            }
        );
    }
}
