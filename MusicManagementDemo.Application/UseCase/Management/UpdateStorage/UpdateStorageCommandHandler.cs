using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MusicManagementDemo.Abstractions.IDbContext;
using MusicManagementDemo.Application.Responses;
using static MusicManagementDemo.Application.Responses.ApiResult<MusicManagementDemo.Application.UseCase.Management.UpdateStorage.UpdateStorageCommandResponse>;

namespace MusicManagementDemo.Application.UseCase.Management.UpdateStorage;

internal sealed class UpdateStorageCommandHandler(
    IManagementAppDbContext dbContext,
    ILogger<UpdateStorageCommandHandler> logger
) : IRequestHandler<UpdateStorageCommand, ApiResult<UpdateStorageCommandResponse>>
{
    public async ValueTask<ApiResult<UpdateStorageCommandResponse>> Handle(
        UpdateStorageCommand request,
        CancellationToken cancellationToken
    )
    {
        var storageToUpdate = await dbContext.Storages.SingleOrDefaultAsync(
            e => e.Id == request.Id,
            cancellationToken: cancellationToken
        );
        if (storageToUpdate is null)
        {
            logger.LogError("storage {id} not found", request.Id);
            return Err(404, "没有找到对应存储");
        }

        storageToUpdate.Name = request.Name;
        storageToUpdate.Path = request.Path;

        try
        {
            if (await dbContext.SaveChangesAsync(cancellationToken) != 1)
            {
                logger.LogError("Update storage failed, {@storageToUpdate}", storageToUpdate);
                return Err(503, "内部错误");
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Update storage failed, {@storageToUpdate}", storageToUpdate);
            return Err(503, "内部错误");
        }

        return Ok(new UpdateStorageCommandResponse());
    }
}
