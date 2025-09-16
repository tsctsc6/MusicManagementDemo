using MediatR;
using Microsoft.EntityFrameworkCore;
using MusicManagementDemo.Abstractions;
using MusicManagementDemo.Abstractions.IDbContext;
using MusicManagementDemo.Application.Responses;

namespace MusicManagementDemo.Application.UseCase.Music.RemoveMusicInfoFromMusicList;

internal sealed class RemoveMusicInfoFromMusicListCommandHandler(IMusicAppDbContext dbContext)
    : IRequestHandler<RemoveMusicInfoFromMusicListCommand, IServiceResult>
{
    public async Task<IServiceResult> Handle(
        RemoveMusicInfoFromMusicListCommand request,
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

        var musicInfoMapToRemove = await dbContext.MusicInfoMusicListMap.SingleOrDefaultAsync(
            e => e.MusicListId == request.MusicListId && e.MusicInfoId == request.MusicInfoId,
            cancellationToken
        );
        if (musicInfoMapToRemove is null)
        {
            return ServiceResult.Err(404, ["musicInfoMapToRemove not found"]);
        }

        await using var transaction = await dbContext.Database.BeginTransactionAsync(
            cancellationToken
        );
        var expectedSubmitCount = 0;

        // 更改目标歌曲的前后歌曲指针
        if (musicInfoMapToRemove.PrevId is not null)
        {
            var musicInfoMapPrev = await dbContext.MusicInfoMusicListMap.SingleOrDefaultAsync(
                e =>
                    e.MusicListId == request.MusicListId
                    && e.MusicInfoId == musicInfoMapToRemove.PrevId,
                cancellationToken
            );
            if (musicInfoMapPrev is null)
            {
                return ServiceResult.Err(404, ["musicInfoMapPrev not found"]);
            }
            musicInfoMapPrev.NextId = musicInfoMapToRemove.NextId;
            expectedSubmitCount++;
        }
        if (musicInfoMapToRemove.NextId is not null)
        {
            var musicInfoMapNext = await dbContext.MusicInfoMusicListMap.SingleOrDefaultAsync(
                e =>
                    e.MusicListId == request.MusicListId
                    && e.MusicInfoId == musicInfoMapToRemove.NextId,
                cancellationToken
            );
            if (musicInfoMapNext is null)
            {
                return ServiceResult.Err(404, ["musicInfoMapNext not found"]);
            }
            musicInfoMapNext.PrevId = musicInfoMapToRemove.PrevId;
            expectedSubmitCount++;
        }
        
        dbContext.MusicInfoMusicListMap.Remove(musicInfoMapToRemove);
        expectedSubmitCount++;
        if (await dbContext.SaveChangesAsync(cancellationToken) != expectedSubmitCount)
        {
            return ServiceResult.Err(404, ["Delete Failed"]);
        }

        await transaction.CommitAsync(cancellationToken);
        return ServiceResult.Ok();
    }
}
