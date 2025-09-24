using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MusicManagementDemo.Abstractions;
using MusicManagementDemo.Abstractions.IDbContext;
using MusicManagementDemo.Application.Responses;

namespace MusicManagementDemo.Application.UseCase.Music.GetAllMusicInfoFromMusicList;

internal sealed class GetAllMusicInfoFromMusicListQueryHandler(
    IMusicAppDbContext dbContext,
    ILogger<GetAllMusicInfoFromMusicListQueryHandler> logger
) : IRequestHandler<GetAllMusicInfoFromMusicListQuery, IServiceResult>
{
    public async Task<IServiceResult> Handle(
        GetAllMusicInfoFromMusicListQuery request,
        CancellationToken cancellationToken
    )
    {
        var musicListToRead = await dbContext
            .MusicList.AsNoTracking()
            .SingleOrDefaultAsync(
                e => e.UserId == request.UserId && e.Id == request.MusicListId,
                cancellationToken: cancellationToken
            );
        if (musicListToRead is null)
        {
            logger.LogError("MusicList {MusicListId} not found", request.MusicListId);
            return ServiceResult.Err(404, ["MusicList not found"]);
        }

        var musicInfosToReadQuery = dbContext
            .MusicInfo.GetMusicInfoInMusicList(
                request.MusicListId,
                request.ReferenceId,
                request.PageSize,
                request.Asc
            )
            .Select(x => new
            {
                x.Id,
                x.Title,
                x.Artist,
                x.Album,
            });
        var musicInfosToRead = await musicInfosToReadQuery.ToArrayAsync(
            cancellationToken: cancellationToken
        );

        return ServiceResult.Ok(
            new { MusicListName = musicListToRead.Name, MusicInfos = musicInfosToRead }
        );
    }
}
