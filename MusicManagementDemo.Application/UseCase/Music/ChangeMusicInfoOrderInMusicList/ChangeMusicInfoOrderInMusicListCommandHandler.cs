using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MusicManagementDemo.Abstractions;
using MusicManagementDemo.Abstractions.IDbContext;
using MusicManagementDemo.Application.Responses;
using MusicManagementDemo.Domain.Entity.Music;
using static MusicManagementDemo.Application.Responses.ApiResult<MusicManagementDemo.Application.UseCase.Music.ChangeMusicInfoOrderInMusicList.ChangeMusicInfoOrderInMusicListCommandResponse>;

namespace MusicManagementDemo.Application.UseCase.Music.ChangeMusicInfoOrderInMusicList;

internal sealed class ChangeMusicInfoOrderInMusicListCommandHandler(
    IMusicAppDbContext dbContext,
    ILexoRankManager lexoRankManager,
    ILogger<ChangeMusicInfoOrderInMusicListCommandHandler> logger
)
    : IRequestHandler<
        ChangeMusicInfoOrderInMusicListCommand,
        ApiResult<ChangeMusicInfoOrderInMusicListCommandResponse>
    >
{
    public async ValueTask<ApiResult<ChangeMusicInfoOrderInMusicListCommandResponse>> Handle(
        ChangeMusicInfoOrderInMusicListCommand request,
        CancellationToken cancellationToken
    )
    {
        if (
            !await dbContext.MusicLists.AnyAsync(
                e => e.Id == request.MusicListId && e.UserId == request.UserId,
                cancellationToken: cancellationToken
            )
        )
        {
            logger.LogError("MusicList {Id} not found", request.MusicListId);
            return Err(404, "MusicListId not found");
        }

        if (
            request.TargetMusicInfoId == request.PrevMusicInfoId
            || request.TargetMusicInfoId == request.NextMusicInfoId
        )
        {
            logger.LogError(
                "request.TargetMusicInfoId == request.PrevMusicInfoId || request.TargetMusicInfoId == request.NextMusicInfoId"
            );
            return Err(404, "args eror");
        }

        MusicInfoMusicListMap? prevItem = null;
        if (request.PrevMusicInfoId is not null)
        {
            prevItem = await dbContext.MusicInfoMusicListMaps.SingleOrDefaultAsync(
                e =>
                    e.MusicListId == request.MusicListId
                    && e.MusicInfoId == request.PrevMusicInfoId,
                cancellationToken
            );
            if (prevItem is null)
            {
                logger.LogError("MusicInfoId {Id} not found", request.PrevMusicInfoId);
                return Err(404, "MusicInfoId not found");
            }
        }
        MusicInfoMusicListMap? nextItem = null;
        if (request.NextMusicInfoId is not null)
        {
            nextItem = await dbContext.MusicInfoMusicListMaps.SingleOrDefaultAsync(
                e =>
                    e.MusicListId == request.MusicListId
                    && e.MusicInfoId == request.NextMusicInfoId,
                cancellationToken
            );
            if (nextItem is null)
            {
                logger.LogError("MusicInfoId {Id} not found", request.NextMusicInfoId);
                return Err(404, "MusicInfoId not found");
            }
        }

        if (!await CheckNeighboringAsync(request, prevItem, nextItem, cancellationToken))
        {
            logger.LogError(
                "Prev MusicInfoMap in new position and Next MusicInfoMap in new position is not neighboring"
            );
            return Err(404, "Invalid input");
        }

        var musicInfoMapToMove = await dbContext.MusicInfoMusicListMaps.SingleOrDefaultAsync(
            e => e.MusicListId == request.MusicListId && e.MusicInfoId == request.TargetMusicInfoId,
            cancellationToken
        );
        if (musicInfoMapToMove is null)
        {
            logger.LogError("Target MusicInfo {Id} not found", request.TargetMusicInfoId);
            return Err(404, "musicInfoMapToMove not found");
        }

        var prevSortingValue = prevItem?.SortingOrder ?? string.Empty;
        var nextSortingValue = nextItem?.SortingOrder ?? string.Empty;
        musicInfoMapToMove.SortingOrder = lexoRankManager.Between(
            prevSortingValue,
            nextSortingValue
        );
        dbContext.MusicInfoMusicListMaps.Update(musicInfoMapToMove);
        if (await dbContext.SaveChangesAsync(cancellationToken) != 1)
        {
            logger.LogError("SubmitCount is not expected. Expected: 1");
            return Err(404, "Move Failed");
        }

        logger.LogInformation(
            "MusicInfo {MusicInfoId} in MusicList {MusicListId} moved.",
            request.TargetMusicInfoId,
            request.MusicListId
        );
        return Ok(new ChangeMusicInfoOrderInMusicListCommandResponse());
    }

    /// <summary>
    /// 检查 request.PrevMusicInfoId 和 request.NextMusicInfoId 是否相邻
    /// 并且 request.PrevMusicInfoId 在前， request.NextMusicInfoId 再后
    /// 如果 request.PrevMusicInfoId is null ， 检查 request.NextMusicInfoId 是否第一个节点
    /// 如果 request.NextMusicInfoId is null ， 检查 request.PrevMusicInfoId 是否最后一个节点
    /// </summary>
    /// <param name="request"></param>
    /// <param name="prevItem"></param>
    /// <param name="nextItem"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>true for neighboring.</returns>
    private async Task<bool> CheckNeighboringAsync(
        ChangeMusicInfoOrderInMusicListCommand request,
        MusicInfoMusicListMap? prevItem,
        MusicInfoMusicListMap? nextItem,
        CancellationToken cancellationToken
    )
    {
        var state = 0;
        if (prevItem is not null)
        {
            state |= 0b10;
        }
        if (nextItem is not null)
        {
            state |= 0b01;
        }

        switch (state)
        {
            case 0b00:
                return true;
            case 0b01:
                var item01 = await dbContext
                    .MusicInfoMusicListMaps.OrderBy(e => e.SortingOrder)
                    .FirstOrDefaultAsync(cancellationToken);
                if (item01 is null)
                    return false;
                return item01.MusicInfoId == nextItem!.MusicInfoId;
            case 0b10:
                var item10 = await dbContext
                    .MusicInfoMusicListMaps.OrderByDescending(e => e.SortingOrder)
                    .FirstOrDefaultAsync(cancellationToken);
                if (item10 is null)
                    return false;
                return item10.MusicInfoId == prevItem!.MusicInfoId;
            case 0b11:
                if (prevItem!.MusicInfoId == nextItem!.MusicInfoId)
                {
                    return false;
                }
                var item11 = await dbContext
                    .MusicInfoMusicListMaps.OrderBy(e => e.SortingOrder)
                    .Where(e =>
                        e.MusicListId == request.MusicListId
                        && string.Compare(e.SortingOrder, prevItem.SortingOrder) >= 0
                    )
                    .Take(2)
                    .ToArrayAsync(cancellationToken);
                if (item11.Length != 2)
                {
                    return false;
                }
                return item11[0].MusicInfoId == prevItem.MusicInfoId
                    && item11[1].MusicInfoId == nextItem.MusicInfoId;
            default:
                return false;
        }
    }
}
