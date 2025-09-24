using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MusicManagementDemo.Abstractions;
using MusicManagementDemo.Abstractions.IDbContext;
using MusicManagementDemo.Application.Responses;

namespace MusicManagementDemo.Application.UseCase.Music.DeleteMusicList;

internal sealed class DeleteMusicListCommandHandler(
    IMusicAppDbContext dbContext,
    ILogger<DeleteMusicListCommandHandler> logger
) : IRequestHandler<DeleteMusicListCommand, IServiceResult>
{
    public async Task<IServiceResult> Handle(
        DeleteMusicListCommand request,
        CancellationToken cancellationToken
    )
    {
        var musicListToDelete = await dbContext
            .MusicLists.Where(e => e.Id == request.MusicListId && e.UserId == request.UserId)
            .SingleOrDefaultAsync(cancellationToken: cancellationToken);
        if (musicListToDelete is null)
        {
            logger.LogError("MusicList {MusicListId} not found", request.MusicListId);
            return ServiceResult.Err(404, ["没有找到对应的歌单"]);
        }
        await using var transaction = await dbContext.Database.BeginTransactionAsync(
            cancellationToken
        );
        // 删除关联的 MusicInfo
        var expectedSubmitCount = await dbContext
            .MusicInfoMusicListMaps.Where(e => e.MusicListId == request.MusicListId)
            .CountAsync(cancellationToken);
        var submitCount = await dbContext
            .MusicInfoMusicListMaps.Where(e => e.MusicListId == request.MusicListId)
            .ExecuteDeleteAsync(cancellationToken);
        if (submitCount != expectedSubmitCount)
        {
            await transaction.RollbackAsync(cancellationToken);
            logger.LogError(
                "Error on delete MusicInfoMusicListMap, expected submit count: {expectedSubmitCount}, in fact {deleteCount}",
                expectedSubmitCount,
                submitCount
            );
            return ServiceResult.Err(503, ["内部错误"]);
        }
        if (
            await dbContext
                .MusicLists.Where(e => e.Id == request.MusicListId && e.UserId == request.UserId)
                .ExecuteDeleteAsync(cancellationToken) != 1
        )
        {
            await transaction.RollbackAsync(cancellationToken);
            logger.LogError("Error in delete MusicList {MusicListId}", request.MusicListId);
            return ServiceResult.Err(503, ["内部错误"]);
        }
        await transaction.CommitAsync(cancellationToken);
        return ServiceResult.Ok();
    }
}
