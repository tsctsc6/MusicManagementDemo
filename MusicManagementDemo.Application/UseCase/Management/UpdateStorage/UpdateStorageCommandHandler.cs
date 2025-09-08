using MediatR;
using Microsoft.EntityFrameworkCore;
using MusicManagementDemo.Infrastructure.Database;
using MusicManagementDemo.Infrastructure.Responses;
using MusicManagementDemo.SharedKernel;

namespace MusicManagementDemo.Application.UseCase.Management.UpdateStorage;

internal sealed class UpdateStorageCommandHandler(ManagementAppDbContext dbContext)
    : IRequestHandler<UpdateStorageCommand, IServiceResult>
{
    public async Task<IServiceResult> Handle(
        UpdateStorageCommand request,
        CancellationToken cancellationToken
    )
    {
        var storageToUpdate = await dbContext.Storage.SingleOrDefaultAsync(
            e => e.Id == request.Id,
            cancellationToken: cancellationToken
        );
        if (storageToUpdate is null)
        {
            return ServiceResult.Err(404, ["没有找到对应存储"]);
        }

        storageToUpdate.Name = request.Name;
        storageToUpdate.Path = request.Path;

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception)
        {
            return ServiceResult.Err(503, ["内部错误"]);
        }

        return ServiceResult.Ok();
    }
}
