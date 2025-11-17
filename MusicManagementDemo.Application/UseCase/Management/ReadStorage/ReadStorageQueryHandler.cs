using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MusicManagementDemo.Abstractions.IDbContext;
using MusicManagementDemo.Application.Responses;

namespace MusicManagementDemo.Application.UseCase.Management.ReadStorage;

internal sealed class ReadStorageQueryHandler(
    IManagementAppDbContext dbContext,
    ILogger<ReadStorageQueryHandler> logger
) : IRequestHandler<ReadStorageQuery, IServiceResult>
{
    public async ValueTask<IServiceResult> Handle(
        ReadStorageQuery request,
        CancellationToken cancellationToken
    )
    {
        var storageToRead = await dbContext
            .Storages.AsNoTracking()
            .SingleOrDefaultAsync(cancellationToken: cancellationToken);
        if (storageToRead is null)
        {
            logger.LogError("storage {id} not found", request.Id);
            return ApiResult<>.Err(503, ["没有找到对应的存储"]);
        }
        return ApiResult<>.Ok(
            new ReadStorageQueryResponse(storageToRead.Id, storageToRead.Name, storageToRead.Path)
        );
    }
}
