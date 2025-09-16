using MediatR;
using Microsoft.EntityFrameworkCore;
using MusicManagementDemo.Abstractions;
using MusicManagementDemo.Abstractions.IDbContext;
using MusicManagementDemo.Application.Responses;
using MusicManagementDemo.Domain.Entity.Music;

namespace MusicManagementDemo.Application.UseCase.Music.ChangeMusicInfoOrderInMusicList;

internal sealed class ChangeMusicInfoOrderInMusicListCommandHandler(IMusicAppDbContext dbContext)
    : IRequestHandler<ChangeMusicInfoOrderInMusicListCommand, IServiceResult>
{
    public async Task<IServiceResult> Handle(
        ChangeMusicInfoOrderInMusicListCommand request,
        CancellationToken cancellationToken
    )
    {
        if (
            !await dbContext.MusicList.AnyAsync(
                e => e.Id == request.MusicListId && e.UserId == request.UserId,
                cancellationToken: cancellationToken
            )
        )
        {
            return ServiceResult.Err(404, ["MusicListId not found"]);
        }

        var musicInfoMapToMove = await dbContext.MusicInfoMusicListMap.SingleOrDefaultAsync(
            e => e.MusicListId == request.MusicListId && e.MusicInfoId == request.TargetMusicInfoId,
            cancellationToken
        );
        if (musicInfoMapToMove is null)
        {
            return ServiceResult.Err(404, ["musicInfoMapToMove not found"]);
        }

        if (!await CheckNeighboringAsync(request, cancellationToken))
        {
            return ServiceResult.Err(404, ["Invalid input"]);
        }

        var rowChanged = new HashSet<Guid>();
        if (
            musicInfoMapToMove.PrevId != request.PrevMusicInfoId
            || musicInfoMapToMove.NextId != request.NextMusicInfoId
        )
        {
            rowChanged.Add(request.TargetMusicInfoId);
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
                return ServiceResult.Err(404, ["musicInfoMapPrevOlds not found"]);
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
                return ServiceResult.Err(404, ["musicInfoMapNextOld not found"]);
            }
            musicInfoMapNextOld.PrevId = musicInfoMapToMove.PrevId;
            rowChanged.Add(musicInfoMapNextOld.MusicInfoId);
        }

        // 更改目标歌曲，新位置的前后歌曲的指针
        if (request.PrevMusicInfoId is not null)
        {
            var musicInfoMapPrevNew = await dbContext.MusicInfoMusicListMap.SingleOrDefaultAsync(
                e =>
                    e.MusicListId == request.MusicListId
                    && e.MusicInfoId == request.PrevMusicInfoId,
                cancellationToken: cancellationToken
            );
            if (musicInfoMapPrevNew is null)
            {
                return ServiceResult.Err(404, ["musicInfoMapPrevNew not found"]);
            }
            musicInfoMapPrevNew.NextId = musicInfoMapToMove.MusicInfoId;
            rowChanged.Add(musicInfoMapPrevNew.MusicInfoId);
        }
        musicInfoMapToMove.PrevId = request.PrevMusicInfoId;

        if (request.NextMusicInfoId is not null)
        {
            var musicInfoMapNextNew = await dbContext.MusicInfoMusicListMap.SingleOrDefaultAsync(
                e =>
                    e.MusicListId == request.MusicListId
                    && e.MusicInfoId == request.NextMusicInfoId,
                cancellationToken: cancellationToken
            );
            if (musicInfoMapNextNew is null)
            {
                return ServiceResult.Err(404, ["musicInfoMapNextNew not found"]);
            }
            musicInfoMapNextNew.PrevId = musicInfoMapToMove.MusicInfoId;
            rowChanged.Add(musicInfoMapNextNew.MusicInfoId);
        }
        musicInfoMapToMove.NextId = request.NextMusicInfoId;

        if (await dbContext.SaveChangesAsync(cancellationToken) != rowChanged.Count)
        {
            return ServiceResult.Err(404, ["Move Failed"]);
        }

        await transaction.CommitAsync(cancellationToken);
        return ServiceResult.Ok();
    }

    /// <summary>
    /// 检查 request.PrevMusicInfoId 和 request.NextMusicInfoId 是否相邻
    /// 并且 request.PrevMusicInfoId 在前， request.NextMusicInfoId 再后
    /// 如果 request.PrevMusicInfoId is null ， 检查 request.NextMusicInfoId 是否第一个节点
    /// 如果 request.NextMusicInfoId is null ， 检查 request.PrevMusicInfoId 是否最后一个节点
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>true for neighboring.</returns>
    private async Task<bool> CheckNeighboringAsync(
        ChangeMusicInfoOrderInMusicListCommand request,
        CancellationToken cancellationToken
    )
    {
        if (request.PrevMusicInfoId is null && request.NextMusicInfoId is null)
        {
            return false;
        }

        MusicInfoMusicListMap? prevMusicInfoMap = null;
        MusicInfoMusicListMap? nextMusicInfoMap = null;
        if (request.PrevMusicInfoId is not null)
        {
            prevMusicInfoMap = await dbContext
                .MusicInfoMusicListMap.Where(e =>
                    e.MusicListId == request.MusicListId && e.MusicInfoId == request.PrevMusicInfoId
                )
                .SingleOrDefaultAsync(cancellationToken: cancellationToken);
        }

        if (request.NextMusicInfoId is not null)
        {
            nextMusicInfoMap = await dbContext
                .MusicInfoMusicListMap.Where(e =>
                    e.MusicListId == request.MusicListId && e.MusicInfoId == request.NextMusicInfoId
                )
                .SingleOrDefaultAsync(cancellationToken: cancellationToken);
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
