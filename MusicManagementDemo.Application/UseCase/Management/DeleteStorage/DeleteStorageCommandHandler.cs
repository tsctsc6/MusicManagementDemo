using MediatR;
using Microsoft.EntityFrameworkCore;
using MusicManagementDemo.Application.Responses;
using MusicManagementDemo.Abstractions;
using MusicManagementDemo.Abstractions.IDbContext;

namespace MusicManagementDemo.Application.UseCase.Management.DeleteStorage;

internal sealed class DeleteStorageCommandHandler(IManagementAppDbContext dbContext)
    : IRequestHandler<DeleteStorageCommand, IServiceResult>
{
    public async Task<IServiceResult> Handle(
        DeleteStorageCommand request,
        CancellationToken cancellationToken
    )
    {
        var storageToDelete = await dbContext.Storage.SingleOrDefaultAsync(
            e => e.Id == request.Id,
            cancellationToken: cancellationToken
        );
        if (storageToDelete is null)
        {
            return ServiceResult.Err(404, ["未找到对应的存储"]);
        }
        dbContext.Storage.Remove(storageToDelete);
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
