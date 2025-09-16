using MediatR;
using Microsoft.EntityFrameworkCore;
using MusicManagementDemo.Abstractions;
using MusicManagementDemo.Abstractions.IDbContext;
using MusicManagementDemo.Application.Responses;
using DbFunctions = MusicManagementDemo.Abstractions.DbFunctions;

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

        var musicInfosToReadQuery = dbContext
            .MusicInfo.FromSqlRaw(
                request.ReferenceId is null
                    ? $"""
                    SELECT * FROM {DbSchemas.Music}.{DbFunctions.GetMusicInfoInMusicList}('{request.MusicListId}'::UUID, NULL::UUID, {request.PageSize}, {!request.Asc})
                    """
                    : $"""
                    SELECT * FROM {DbSchemas.Music}.{DbFunctions.GetMusicInfoInMusicList}('{request.MusicListId}'::UUID, '{request.ReferenceId}'::UUID, {request.PageSize}, {!request.Asc})
                    """
            )
            .Select(x => new
            {
                x.Id,
                x.Title,
                x.Artist,
                x.Album,
            });
        var musicInfosToRead = musicInfosToReadQuery.ToArrayAsync(
            cancellationToken: cancellationToken
        );

        return ServiceResult.Ok(
            new { MusicListName = musicListToRead.Name, MusicInfos = musicInfosToReadQuery }
        );
    }
}
