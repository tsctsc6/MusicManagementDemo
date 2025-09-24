using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MusicManagementDemo.Abstractions;
using MusicManagementDemo.Abstractions.IDbContext;
using MusicManagementDemo.Application.Responses;

namespace MusicManagementDemo.Application.UseCase.Music.RemoveMusicInfoFromMusicList;

internal sealed class RemoveMusicInfoFromMusicListCommandHandler(
    IMusicAppDbContext dbContext,
    ILogger<RemoveMusicInfoFromMusicListCommandHandler> logger
) : IRequestHandler<RemoveMusicInfoFromMusicListCommand, IServiceResult>
{
    public async Task<IServiceResult> Handle(
        RemoveMusicInfoFromMusicListCommand request,
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
            logger.LogError("MusicList {MusicListId} not found", request.MusicListId);
            return ServiceResult.Err(404, ["MusicList not found"]);
        }

        var musicInfoMapToRemove = await dbContext.MusicInfoMusicListMaps.SingleOrDefaultAsync(
            e => e.MusicListId == request.MusicListId && e.MusicInfoId == request.MusicInfoId,
            cancellationToken
        );
        if (musicInfoMapToRemove is null)
        {
            logger.LogError("MusicInfo {MusicInfoId} not found", request.MusicInfoId);
            return ServiceResult.Err(404, ["musicInfoMapToRemove not found"]);
        }

        await using var transaction = await dbContext.Database.BeginTransactionAsync(
            cancellationToken
        );
        var expectedSubmitCount = 0;

        // 更改目标歌曲的前后歌曲指针
        if (musicInfoMapToRemove.PrevId is not null)
        {
            var musicInfoMapPrev = await dbContext.MusicInfoMusicListMaps.SingleOrDefaultAsync(
                e =>
                    e.MusicListId == request.MusicListId
                    && e.MusicInfoId == musicInfoMapToRemove.PrevId,
                cancellationToken
            );
            if (musicInfoMapPrev is null)
            {
                logger.LogError(
                    "Prev MusicInfo {MusicInfoId} not found",
                    musicInfoMapToRemove.PrevId
                );
                return ServiceResult.Err(404, ["musicInfoMapPrev not found"]);
            }
            musicInfoMapPrev.NextId = musicInfoMapToRemove.NextId;
            expectedSubmitCount++;
        }
        if (musicInfoMapToRemove.NextId is not null)
        {
            var musicInfoMapNext = await dbContext.MusicInfoMusicListMaps.SingleOrDefaultAsync(
                e =>
                    e.MusicListId == request.MusicListId
                    && e.MusicInfoId == musicInfoMapToRemove.NextId,
                cancellationToken
            );
            if (musicInfoMapNext is null)
            {
                logger.LogError(
                    "Next MusicInfo {MusicInfoId} not found",
                    musicInfoMapToRemove.NextId
                );
                return ServiceResult.Err(404, ["musicInfoMapNext not found"]);
            }
            musicInfoMapNext.PrevId = musicInfoMapToRemove.PrevId;
            expectedSubmitCount++;
        }

        dbContext.MusicInfoMusicListMaps.Remove(musicInfoMapToRemove);
        expectedSubmitCount++;
        var submitCount = await dbContext.SaveChangesAsync(cancellationToken);
        if (submitCount != expectedSubmitCount)
        {
            logger.LogError(
                "Delete Failed, expected submit count: {expectedSubmitCount}, submit count: {submitCount}",
                expectedSubmitCount,
                submitCount
            );
            return ServiceResult.Err(404, ["Delete Failed"]);
        }

        await transaction.CommitAsync(cancellationToken);
        logger.LogInformation("MusicInfoMap {Id} Removed", musicInfoMapToRemove.MusicInfoId);
        return ServiceResult.Ok();
    }
}
