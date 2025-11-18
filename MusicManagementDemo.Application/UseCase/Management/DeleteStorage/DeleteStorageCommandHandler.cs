using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MusicManagementDemo.Abstractions.IDbContext;
using MusicManagementDemo.Application.Responses;
using static MusicManagementDemo.Application.Responses.ApiResult<MusicManagementDemo.Application.UseCase.Management.DeleteStorage.DeleteStorageCommandResponse>;

namespace MusicManagementDemo.Application.UseCase.Management.DeleteStorage;

internal sealed class DeleteStorageCommandHandler(
    IManagementAppDbContext dbContext,
    ILogger<DeleteStorageCommandHandler> logger
) : IRequestHandler<DeleteStorageCommand, ApiResult<DeleteStorageCommandResponse>>
{
    public async ValueTask<ApiResult<DeleteStorageCommandResponse>> Handle(
        DeleteStorageCommand request,
        CancellationToken cancellationToken
    )
    {
        var storageToDelete = await dbContext.Storages.SingleOrDefaultAsync(
            e => e.Id == request.Id,
            cancellationToken: cancellationToken
        );
        if (storageToDelete is null)
        {
            logger.LogError("Storage with id: {RequestId} not found", request.Id);
            return Err(404, "未找到对应的存储");
        }
        dbContext.Storages.Remove(storageToDelete);
        try
        {
            if (await dbContext.SaveChangesAsync(cancellationToken) != 1)
            {
                logger.LogError("Error during delete storage: {@storageToDelete}", storageToDelete);
                return Err(503, "内部错误");
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error during delete storage: {@storageToDelete}", storageToDelete);
            return Err(503, "内部错误");
        }
        return Ok(new DeleteStorageCommandResponse());
    }
}
