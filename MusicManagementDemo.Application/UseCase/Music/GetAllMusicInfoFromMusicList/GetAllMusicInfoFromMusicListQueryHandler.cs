using MediatR;
using Microsoft.EntityFrameworkCore;
using MusicManagementDemo.Abstractions;
using MusicManagementDemo.Abstractions.IDbContext;
using MusicManagementDemo.Application.Responses;

namespace MusicManagementDemo.Application.UseCase.Music.GetAllMusicInfoFromMusicList;

internal sealed class GetAllMusicInfoFromMusicListQueryHandler(IMusicAppDbContext dbContext)
    : IRequestHandler<GetAllMusicInfoFromMusicListQuery, IServiceResult>
{
    public async Task<IServiceResult> Handle(
        GetAllMusicInfoFromMusicListQuery request,
        CancellationToken cancellationToken
    )
    {
        var musicListToRead = await dbContext
            .MusicList.AsNoTracking()
            .SingleOrDefaultAsync(
                e => e.Id == request.MusicListId,
                cancellationToken: cancellationToken
            );
        if (musicListToRead is null)
        {
            return ServiceResult.Err(404, ["MusicList not found"]);
        }
        var musicInfosToReadQuery =
            from m in dbContext.MusicInfoMusicListMap.Where(e =>
                e.MusicListId == request.MusicListId
            )
            join mi in dbContext.MusicInfo on m.MusicInfoId equals mi.Id
            select new
            {
                mi.Id,
                mi.Title,
                mi.Artist,
                mi.Album,
            };
        if (request.Asc)
        {
            musicInfosToReadQuery = musicInfosToReadQuery.OrderBy(e => e.Id);
            if (request.ReferenceId is not null)
            {
                musicInfosToReadQuery = musicInfosToReadQuery.Where(e =>
                    e.Id > request.ReferenceId
                );
            }
        }
        else
        {
            musicInfosToReadQuery = musicInfosToReadQuery.OrderByDescending(e => e.Id);
            if (request.ReferenceId is not null)
            {
                musicInfosToReadQuery = musicInfosToReadQuery.Where(e =>
                    e.Id < request.ReferenceId
                );
            }
        }
        var musicInfosToRead = await musicInfosToReadQuery
            .Take(request.PageSize)
            .AsNoTracking()
            .ToArrayAsync(cancellationToken: cancellationToken);
        return ServiceResult.Ok(
            new { MusicListName = musicListToRead.Name, MusicInfos = musicInfosToRead }
        );
    }
}
