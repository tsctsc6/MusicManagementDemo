using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MusicManagementDemo.Abstractions;
using MusicManagementDemo.Abstractions.IDbContext;
using MusicManagementDemo.Application.Responses;
using MusicManagementDemo.Domain.Entity.Music;

namespace MusicManagementDemo.Application.UseCase.Music.ChangeMusicInfoOrderInMusicList;

internal sealed class ChangeMusicInfoOrderInMusicListCommandHandler(
    IMusicAppDbContext dbContext,
    ILogger<ChangeMusicInfoOrderInMusicListCommandHandler> logger
) : IRequestHandler<ChangeMusicInfoOrderInMusicListCommand, IServiceResult>
{
    public async Task<IServiceResult> Handle(
        ChangeMusicInfoOrderInMusicListCommand request,
        CancellationToken cancellationToken
    )
    {
        logger.LogInformation("Request args: {@request}", request);
        if (
            !await dbContext.MusicList.AnyAsync(
                e => e.Id == request.MusicListId && e.UserId == request.UserId,
                cancellationToken: cancellationToken
            )
        )
        {
            logger.LogError("MusicList {Id} not found", request.MusicListId);
            return ServiceResult.Err(404, ["MusicListId not found"]);
        }

        var musicInfoMapToMove = await dbContext.MusicInfoMusicListMap.SingleOrDefaultAsync(
            e => e.MusicListId == request.MusicListId && e.MusicInfoId == request.TargetMusicInfoId,
            cancellationToken
        );
        if (musicInfoMapToMove is null)
        {
            logger.LogError("Target MusicInfo {Id} not found", request.TargetMusicInfoId);
            return ServiceResult.Err(404, ["musicInfoMapToMove not found"]);
        }

        var rowChanged = new HashSet<Guid>();
        if (
            musicInfoMapToMove.PrevId != request.PrevMusicInfoId
            || musicInfoMapToMove.NextId != request.NextMusicInfoId
        )
        {
            rowChanged.Add(request.TargetMusicInfoId);
        }

        // 查询目标歌曲，新位置的前后歌曲
        MusicInfoMusicListMap? musicInfoMapPrevNew = null;
        MusicInfoMusicListMap? musicInfoMapNextNew = null;
        if (request.PrevMusicInfoId is not null)
        {
            musicInfoMapPrevNew = await dbContext.MusicInfoMusicListMap.SingleOrDefaultAsync(
                e =>
                    e.MusicListId == request.MusicListId
                    && e.MusicInfoId == request.PrevMusicInfoId,
                cancellationToken: cancellationToken
            );
            if (musicInfoMapPrevNew is null)
            {
                logger.LogError("Prev MusicInfoMap in new position not find");
                return ServiceResult.Err(404, ["musicInfoMapPrevNew not found"]);
            }
        }

        if (request.NextMusicInfoId is not null)
        {
            musicInfoMapNextNew = await dbContext.MusicInfoMusicListMap.SingleOrDefaultAsync(
                e =>
                    e.MusicListId == request.MusicListId
                    && e.MusicInfoId == request.NextMusicInfoId,
                cancellationToken: cancellationToken
            );
            if (musicInfoMapNextNew is null)
            {
                logger.LogError("Next MusicInfoMap in new position not find");
                return ServiceResult.Err(404, ["musicInfoMapNextNew not found"]);
            }
        }
        logger.LogInformation(
            "Prev MusicInfoMap in new position: {@musicInfoMapPrevNew}",
            musicInfoMapPrevNew
        );
        logger.LogInformation(
            "Next MusicInfoMap in new position: {@musicInfoMapNextNew}",
            musicInfoMapNextNew
        );

        if (!CheckNeighboringAsync(request, musicInfoMapPrevNew, musicInfoMapNextNew))
        {
            logger.LogError(
                "Prev MusicInfoMap in new position and Next MusicInfoMap in new position is not neighboring"
            );
            return ServiceResult.Err(404, ["Invalid input"]);
        }

        await using var transaction = await dbContext.Database.BeginTransactionAsync(
            cancellationToken
        );

        // 更改目标歌曲，旧位置的前后歌曲的指针
        if (musicInfoMapToMove.PrevId is not null)
        {
            var musicInfoMapPrevOld = await dbContext.MusicInfoMusicListMap.SingleOrDefaultAsync(
                e =>
                    e.MusicListId == request.MusicListId
                    && e.MusicInfoId == musicInfoMapToMove.PrevId,
                cancellationToken
            );
            if (musicInfoMapPrevOld is null)
            {
                logger.LogError("Prev MusicInfoMap in old position not find");
                return ServiceResult.Err(404, ["musicInfoMapPrevOld not found"]);
            }
            musicInfoMapPrevOld.NextId = musicInfoMapToMove.NextId;
            rowChanged.Add(musicInfoMapPrevOld.MusicInfoId);
        }
        if (musicInfoMapToMove.NextId is not null)
        {
            var musicInfoMapNextOld = await dbContext.MusicInfoMusicListMap.SingleOrDefaultAsync(
                e =>
                    e.MusicListId == request.MusicListId
                    && e.MusicInfoId == musicInfoMapToMove.NextId,
                cancellationToken
            );
            if (musicInfoMapNextOld is null)
            {
                logger.LogError("Next MusicInfoMap in old position not find");
                return ServiceResult.Err(404, ["musicInfoMapNextOld not found"]);
            }
            musicInfoMapNextOld.PrevId = musicInfoMapToMove.PrevId;
            rowChanged.Add(musicInfoMapNextOld.MusicInfoId);
        }

        // 更改目标歌曲，新位置的前后歌曲的指针
        if (musicInfoMapPrevNew is not null)
        {
            musicInfoMapPrevNew.NextId = musicInfoMapToMove.MusicInfoId;
            rowChanged.Add(musicInfoMapPrevNew.MusicInfoId);
        }
        if (musicInfoMapNextNew is not null)
        {
            musicInfoMapNextNew.PrevId = musicInfoMapToMove.MusicInfoId;
            rowChanged.Add(musicInfoMapNextNew.MusicInfoId);
        }
        musicInfoMapToMove.PrevId = request.PrevMusicInfoId;
        musicInfoMapToMove.NextId = request.NextMusicInfoId;
        rowChanged.Add(musicInfoMapToMove.MusicInfoId);

        var submitCount = await dbContext.SaveChangesAsync(cancellationToken);
        if (submitCount != rowChanged.Count)
        {
            logger.LogError(
                "Expected submit: {rowChangedCount}, submit in realy: {submitCount}",
                rowChanged.Count,
                submitCount
            );
            return ServiceResult.Err(404, ["Move Failed"]);
        }

        await transaction.CommitAsync(cancellationToken);

        logger.LogInformation(
            "MusicInfo {MisicInfoId} in MisicList {MusicListId} moved.",
            request.TargetMusicInfoId,
            request.MusicListId
        );
        return ServiceResult.Ok();
    }

    /// <summary>
    /// 检查 request.PrevMusicInfoId 和 request.NextMusicInfoId 是否相邻
    /// 并且 request.PrevMusicInfoId 在前， request.NextMusicInfoId 再后
    /// 如果 request.PrevMusicInfoId is null ， 检查 request.NextMusicInfoId 是否第一个节点
    /// 如果 request.NextMusicInfoId is null ， 检查 request.PrevMusicInfoId 是否最后一个节点
    /// </summary>
    /// <param name="request"></param>
    /// <param name="prevMusicInfoMap"></param>
    /// <param name="nextMusicInfoMap"></param>
    /// <returns>true for neighboring.</returns>
    private bool CheckNeighboringAsync(
        ChangeMusicInfoOrderInMusicListCommand request,
        MusicInfoMusicListMap? prevMusicInfoMap,
        MusicInfoMusicListMap? nextMusicInfoMap
    )
    {
        if (request.PrevMusicInfoId is null && request.NextMusicInfoId is null)
        {
            return false;
        }

        if (
            request.PrevMusicInfoId is null
            && nextMusicInfoMap is not null
            && nextMusicInfoMap.PrevId is null
        )
            return true;
        if (
            request.NextMusicInfoId is null
            && prevMusicInfoMap is not null
            && prevMusicInfoMap.NextId is null
        )
            return true;

        if (
            prevMusicInfoMap is not null
            && nextMusicInfoMap is not null
            && prevMusicInfoMap.NextId == nextMusicInfoMap.MusicInfoId
            && nextMusicInfoMap.PrevId == prevMusicInfoMap.MusicInfoId
        )
            return true;
        return false;
    }
}
