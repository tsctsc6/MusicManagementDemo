using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MusicManagementDemo.Abstractions.IDbContext;
using MusicManagementDemo.Application.Responses;
using MusicManagementDemo.Domain.Entity.Music;
using static MusicManagementDemo.Application.Responses.ApiResult<MusicManagementDemo.Application.UseCase.Music.GetAllMusicInfoFromMusicList.GetAllMusicInfoFromMusicListQueryResponse>;

namespace MusicManagementDemo.Application.UseCase.Music.GetAllMusicInfoFromMusicList;

internal sealed class GetAllMusicInfoFromMusicListQueryHandler(
    IMusicAppDbContext dbContext,
    ILogger<GetAllMusicInfoFromMusicListQueryHandler> logger
)
    : IRequestHandler<
        GetAllMusicInfoFromMusicListQuery,
        ApiResult<GetAllMusicInfoFromMusicListQueryResponse>
    >
{
    public async ValueTask<ApiResult<GetAllMusicInfoFromMusicListQueryResponse>> Handle(
        GetAllMusicInfoFromMusicListQuery request,
        CancellationToken cancellationToken
    )
    {
        var musicListToRead = await dbContext
            .MusicLists.AsNoTracking()
            .SingleOrDefaultAsync(
                e => e.UserId == request.UserId && e.Id == request.MusicListId,
                cancellationToken: cancellationToken
            );
        if (musicListToRead is null)
        {
            logger.LogError("MusicList {MusicListId} not found", request.MusicListId);
            return Err(404, "MusicList not found");
        }

        var beginSortingValue = string.Empty;
        if (request.ReferenceId is not null)
        {
            beginSortingValue =
                (
                    await dbContext.MusicInfoMusicListMaps.SingleOrDefaultAsync(
                        e => e.MusicInfoId == request.ReferenceId,
                        cancellationToken
                    )
                )?.SortingOrder
                ?? string.Empty;
        }

        IQueryable<MusicInfoMusicListMap> musicInfosToReadQuery_Map =
            dbContext.MusicInfoMusicListMaps;
        if (request.Asc)
        {
            musicInfosToReadQuery_Map = musicInfosToReadQuery_Map.OrderBy(e => e.SortingOrder);
        }
        else
        {
            musicInfosToReadQuery_Map = musicInfosToReadQuery_Map.OrderByDescending(e =>
                e.SortingOrder
            );
        }

        if (string.IsNullOrEmpty(beginSortingValue))
        {
            musicInfosToReadQuery_Map = musicInfosToReadQuery_Map.Where(e =>
                e.MusicListId == request.MusicListId
            );
        }
        else
        {
            musicInfosToReadQuery_Map = musicInfosToReadQuery_Map.Where(e =>
                e.MusicListId == request.MusicListId
                && string.Compare(e.SortingOrder, beginSortingValue) >= 0
            );
        }

        var musicInfosToRead = await (
            from musicInfoMusicListMap in musicInfosToReadQuery_Map.Take(request.PageSize)
            join musicInfo in dbContext.MusicInfos
                on musicInfoMusicListMap.MusicInfoId equals musicInfo.Id
            select new GetAllMusicInfoFromMusicListQueryResponseMusicInfo(
                musicInfo.Id,
                musicInfo.Title,
                musicInfo.Artist,
                musicInfo.Album
            )
        ).ToArrayAsync(cancellationToken: cancellationToken);

        return Ok(
            new GetAllMusicInfoFromMusicListQueryResponse(musicListToRead.Name, musicInfosToRead)
        );
    }
}
