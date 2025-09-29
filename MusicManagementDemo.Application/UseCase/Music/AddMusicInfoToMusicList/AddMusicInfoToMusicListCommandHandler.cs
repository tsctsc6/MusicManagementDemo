using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MusicManagementDemo.Abstractions;
using MusicManagementDemo.Abstractions.IDbContext;
using MusicManagementDemo.Application.Responses;
using MusicManagementDemo.Domain.Entity.Music;

namespace MusicManagementDemo.Application.UseCase.Music.AddMusicInfoToMusicList;

/// <summary>
/// 添加一首歌曲到歌单的末尾。
/// </summary>
/// <param name="dbContext"></param>
internal sealed class AddMusicInfoToMusicListCommandHandler(
    IMusicAppDbContext dbContext,
    ILogger<AddMusicInfoToMusicListCommandHandler> logger
) : IRequestHandler<AddMusicInfoToMusicListCommand, IServiceResult>
{
    public async Task<IServiceResult> Handle(
        AddMusicInfoToMusicListCommand request,
        CancellationToken cancellationToken
    )
    {
        var musicListWhichAddMusicInfo = await dbContext.MusicLists.SingleOrDefaultAsync(
            e => e.Id == request.MusicListId && e.UserId == request.UserId,
            cancellationToken: cancellationToken
        );
        if (musicListWhichAddMusicInfo is null)
        {
            logger.LogError("MusicList {musicListId} not found", request.MusicListId);
            return ServiceResult.Err(404, ["没有找到歌单"]);
        }

        // 歌单中是否存在该歌曲
        if (
            await dbContext.MusicInfoMusicListMaps.AnyAsync(
                e => e.MusicListId == request.MusicListId && e.MusicInfoId == request.MusicInfoId,
                cancellationToken: cancellationToken
            )
        )
        {
            logger.LogError("MusicInfo {musicInfoId} already exist", request.MusicInfoId);
            return ServiceResult.Err(404, ["该歌曲已存在该歌单中"]);
        }

        // 歌曲是否存在
        if (
            await dbContext.MusicInfos.AnyAsync(
                e => e.Id == request.MusicInfoId,
                cancellationToken: cancellationToken
            )
        )
        {
            logger.LogError("MusicInfo {musicInfoId} not exist", request.MusicInfoId);
            return ServiceResult.Err(404, ["该歌曲不存在"]);
        }

        // 查询歌单最后的歌曲
        var lastMusicInfoMap = await dbContext
            .MusicInfoMusicListMaps.Where(e =>
                e.MusicListId == request.MusicListId && e.NextId == null
            )
            .SingleOrDefaultAsync(cancellationToken: cancellationToken);
        var expectedSubmitCount = 0;
        await using var transaction = await dbContext.Database.BeginTransactionAsync(
            cancellationToken
        );
        var musicInfoMapToAdd = new MusicInfoMusicListMap
        {
            MusicListId = request.MusicListId,
            MusicInfoId = request.MusicInfoId,
        };
        if (lastMusicInfoMap is not null)
        {
            lastMusicInfoMap.NextId = request.MusicInfoId;
            dbContext.MusicInfoMusicListMaps.Update(lastMusicInfoMap);
            musicInfoMapToAdd.PrevId = lastMusicInfoMap.MusicInfoId;
            expectedSubmitCount++;
        }

        await dbContext.MusicInfoMusicListMaps.AddAsync(musicInfoMapToAdd, cancellationToken);
        expectedSubmitCount++;
        var submitCount = await dbContext.SaveChangesAsync(cancellationToken);
        if (submitCount != expectedSubmitCount)
        {
            logger.LogError(
                "submitCount is not expected. Expected: {expectedSubmitCount}, in reality: {reality}",
                expectedSubmitCount,
                submitCount
            );
            return ServiceResult.Err(503, ["添加歌曲失败"]);
        }

        await transaction.CommitAsync(cancellationToken);
        logger.LogInformation(
            "MusicInfo {MusicInfoId} added to MusicList {MusicListId}",
            request.MusicInfoId,
            request.MusicListId
        );
        return ServiceResult.Ok();
    }
}
