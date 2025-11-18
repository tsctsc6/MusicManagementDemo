using Mediator;
using Microsoft.EntityFrameworkCore;
using MusicManagementDemo.Abstractions.IDbContext;
using MusicManagementDemo.Application.Responses;
using static MusicManagementDemo.Application.Responses.ApiResult<MusicManagementDemo.Application.UseCase.Music.ReadAllMusicList.ReadAllMusicListQueryResponse>;

namespace MusicManagementDemo.Application.UseCase.Music.ReadAllMusicList;

internal sealed class ReadAllMusicListQueryHandler(IMusicAppDbContext dbContext)
    : IRequestHandler<ReadAllMusicListQuery, ApiResult<ReadAllMusicListQueryResponse>>
{
    public async ValueTask<ApiResult<ReadAllMusicListQueryResponse>> Handle(
        ReadAllMusicListQuery request,
        CancellationToken cancellationToken
    )
    {
        var musicListsToReadQuery = dbContext.MusicLists.AsQueryable();
        if (request.Asc)
        {
            musicListsToReadQuery = musicListsToReadQuery.OrderBy(e => e.Id);
            if (request.ReferenceId is not null)
            {
                musicListsToReadQuery = musicListsToReadQuery.Where(e =>
                    e.Id > request.ReferenceId
                );
            }
        }
        else
        {
            musicListsToReadQuery = musicListsToReadQuery.OrderByDescending(e => e.Id);
            if (request.ReferenceId is not null)
            {
                musicListsToReadQuery = musicListsToReadQuery.Where(e =>
                    e.Id < request.ReferenceId
                );
            }
        }
        var musicListsToRead = await musicListsToReadQuery
            .Where(e => e.UserId == request.UserId)
            .Take(request.PageSize)
            .AsNoTracking()
            .Select(e => new ReadAllMusicListQueryResponseItem(e.Id, e.Name, e.CreatedAt))
            .ToArrayAsync(cancellationToken: cancellationToken);
        return Ok(new ReadAllMusicListQueryResponse(musicListsToRead));
    }
}
