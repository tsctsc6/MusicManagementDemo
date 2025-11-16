using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MusicManagementDemo.Abstractions;
using MusicManagementDemo.Abstractions.IDbContext;
using MusicManagementDemo.Application.Responses;

namespace MusicManagementDemo.Application.UseCase.Management.DeleteStorage;

internal sealed class DeleteStorageCommandHandler(
    IManagementAppDbContext dbContext,
    ILogger<DeleteStorageCommandHandler> logger
) : IRequestHandler<DeleteStorageCommand, IServiceResult>
{
    public async ValueTask<IServiceResult> Handle(
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
            return ServiceResult.Err(404, ["未找到对应的存储"]);
        }
        dbContext.Storages.Remove(storageToDelete);
        try
        {
            if (await dbContext.SaveChangesAsync(cancellationToken) != 1)
            {
                logger.LogError("Error during delete storage: {@storageToDelete}", storageToDelete);
                return ServiceResult.Err(503, ["内部错误"]);
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error during delete storage: {@storageToDelete}", storageToDelete);
            return ServiceResult.Err(503, ["内部错误"]);
        }
        return ServiceResult.Ok(new DeleteStorageCommandResponse());
    }
}
