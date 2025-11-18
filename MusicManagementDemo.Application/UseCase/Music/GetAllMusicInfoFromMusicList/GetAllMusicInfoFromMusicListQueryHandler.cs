using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MusicManagementDemo.Abstractions.IDbContext;
using MusicManagementDemo.Application.Responses;
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

        var musicInfosToReadQuery = dbContext
            .GetMusicInfoInMusicList(
                request.MusicListId,
                request.ReferenceId,
                request.PageSize,
                request.Asc
            )
            .Select(x => new GetAllMusicInfoFromMusicListQueryResponseMusicInfo(
                x.Id,
                x.Title,
                x.Artist,
                x.Album
            ));
        var musicInfosToRead = await musicInfosToReadQuery.ToArrayAsync(
            cancellationToken: cancellationToken
        );

        return Ok(
            new GetAllMusicInfoFromMusicListQueryResponse(musicListToRead.Name, musicInfosToRead)
        );
    }
}
