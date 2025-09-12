using MediatR;
using Microsoft.EntityFrameworkCore;
using MusicManagementDemo.Abstractions;
using MusicManagementDemo.Abstractions.IDbContext;
using MusicManagementDemo.Application.Responses;

namespace MusicManagementDemo.Application.UseCase.Music.DeleteMusicList;

internal sealed class DeleteMusicListCommandHandler(IMusicAppDbContext dbContext)
    : IRequestHandler<DeleteMusicListCommand, IServiceResult>
{
    public async Task<IServiceResult> Handle(
        DeleteMusicListCommand request,
        CancellationToken cancellationToken
    )
    {
        using var transaction = dbContext.Database.BeginTransaction();
        var deleteCount = await dbContext
            .MusicInfoMusicListMap.Where(e => e.MusicListId == request.MusicListId)
            .CountAsync(cancellationToken);
        var submits = await dbContext
            .MusicInfoMusicListMap.Where(e => e.MusicListId == request.MusicListId)
            .ExecuteDeleteAsync(cancellationToken);
        if (submits != deleteCount)
        {
            transaction.Rollback();
            return ServiceResult.Err(503, ["内部错误"]);
        }
        if (
            await dbContext
                .MusicList.Where(e => e.Id == request.MusicListId)
                .ExecuteDeleteAsync(cancellationToken) != 1
        )
        {
            transaction.Rollback();
            return ServiceResult.Err(503, ["内部错误"]);
        }
        await transaction.CommitAsync(cancellationToken);
        return ServiceResult.Ok();
    }
}
