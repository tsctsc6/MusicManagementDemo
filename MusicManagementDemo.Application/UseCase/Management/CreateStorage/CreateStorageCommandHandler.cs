using Mediator;
using Microsoft.Extensions.Logging;
using MusicManagementDemo.Abstractions;
using MusicManagementDemo.Abstractions.IDbContext;
using MusicManagementDemo.Application.Responses;
using MusicManagementDemo.Domain.Entity.Management;

namespace MusicManagementDemo.Application.UseCase.Management.CreateStorage;

internal sealed class CreateStorageCommandHandler(
    IManagementAppDbContext dbContext,
    ILogger<CreateStorageCommandHandler> logger
) : IRequestHandler<CreateStorageCommand, IServiceResult>
{
    public async ValueTask<IServiceResult> Handle(
        CreateStorageCommand request,
        CancellationToken cancellationToken
    )
    {
        var storageToCreate = new Storage { Name = request.Name, Path = request.Path };
        await dbContext.Storages.AddAsync(storageToCreate, cancellationToken);
        try
        {
            if (await dbContext.SaveChangesAsync(cancellationToken) != 1)
            {
                logger.LogError("Error creating storage: {@storageToCreate}", storageToCreate);
                return ServiceResult.Err(503, ["内部错误"]);
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error creating storage: {@storageToCreate}", storageToCreate);
            return ServiceResult.Err(503, ["内部错误"]);
        }
        logger.LogInformation("Created storage: {@storageToCreate}", storageToCreate);
        return ServiceResult.Ok(new CreateStorageCommandResponse(storageToCreate.Id));
    }
}
