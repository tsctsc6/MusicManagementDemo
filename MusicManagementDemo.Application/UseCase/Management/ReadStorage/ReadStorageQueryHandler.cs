using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MusicManagementDemo.Abstractions.IDbContext;
using MusicManagementDemo.Application.Responses;
using static MusicManagementDemo.Application.Responses.ApiResult<MusicManagementDemo.Application.UseCase.Management.ReadStorage.ReadStorageQueryResponse>;

namespace MusicManagementDemo.Application.UseCase.Management.ReadStorage;

internal sealed class ReadStorageQueryHandler(
    IManagementAppDbContext dbContext,
    ILogger<ReadStorageQueryHandler> logger
) : IRequestHandler<ReadStorageQuery, ApiResult<ReadStorageQueryResponse>>
{
    public async ValueTask<ApiResult<ReadStorageQueryResponse>> Handle(
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
            return Err(503, "没有找到对应的存储");
        }
        return Ok(
            new ReadStorageQueryResponse(storageToRead.Id, storageToRead.Name, storageToRead.Path)
        );
    }
}
