using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MusicManagementDemo.Abstractions;
using MusicManagementDemo.Abstractions.IDbContext;
using MusicManagementDemo.Application.Responses;
using MusicManagementDemo.Domain.Entity.Music;
using static MusicManagementDemo.Application.Responses.ApiResult<MusicManagementDemo.Application.UseCase.Music.AddMusicInfoToMusicList.AddMusicInfoToMusicListCommandResponse>;

namespace MusicManagementDemo.Application.UseCase.Music.AddMusicInfoToMusicList;

/// <summary>
/// 添加一首歌曲到歌单的末尾。
/// </summary>
/// <param name="dbContext"></param>
internal sealed class AddMusicInfoToMusicListCommandHandler(
    IMusicAppDbContext dbContext,
    ILexoRankManager lexoRankManager,
    ILogger<AddMusicInfoToMusicListCommandHandler> logger
)
    : IRequestHandler<
        AddMusicInfoToMusicListCommand,
        ApiResult<AddMusicInfoToMusicListCommandResponse>
    >
{
    public async ValueTask<ApiResult<AddMusicInfoToMusicListCommandResponse>> Handle(
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
            return Err(404, "没有找到歌单");
        }

        // 歌曲是否存在
        if (
            !await dbContext.MusicInfos.AnyAsync(
                e => e.Id == request.MusicInfoId,
                cancellationToken: cancellationToken
            )
        )
        {
            logger.LogError("MusicInfo {musicInfoId} not exist", request.MusicInfoId);
            return Err(404, "该歌曲不存在");
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
            return Err(404, "该歌曲已存在该歌单中");
        }

        // 查询歌单最后的歌曲
        var lastMusicInfoMap = await dbContext
            .MusicInfoMusicListMaps.AsNoTracking()
            .OrderByDescending(e => e.SortingOrder)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        await using var transaction = await dbContext.Database.BeginTransactionAsync(
            cancellationToken
        );

        var lastSortingValue = string.Empty;
        if (lastMusicInfoMap is not null)
        {
            lastSortingValue = lastMusicInfoMap.SortingOrder;
        }
        var sortingValue = lexoRankManager.Between(lastSortingValue, string.Empty);

        var musicInfoMapToAdd = new MusicInfoMusicListMap
        {
            MusicListId = request.MusicListId,
            MusicInfoId = request.MusicInfoId,
            SortingOrder = sortingValue,
        };

        await dbContext.MusicInfoMusicListMaps.AddAsync(musicInfoMapToAdd, cancellationToken);
        if (await dbContext.SaveChangesAsync(cancellationToken) != 1)
        {
            logger.LogError("SubmitCount is not expected. Expected: 1");
            return Err(503, "添加歌曲失败");
        }

        await transaction.CommitAsync(cancellationToken);
        logger.LogInformation(
            "MusicInfo {MusicInfoId} added to MusicList {MusicListId}",
            request.MusicInfoId,
            request.MusicListId
        );
        return Ok(new AddMusicInfoToMusicListCommandResponse());
    }
}
