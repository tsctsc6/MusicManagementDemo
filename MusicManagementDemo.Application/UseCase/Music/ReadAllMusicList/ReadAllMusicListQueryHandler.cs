using MediatR;
using Microsoft.EntityFrameworkCore;
using MusicManagementDemo.Abstractions;
using MusicManagementDemo.Abstractions.IDbContext;
using MusicManagementDemo.Application.Responses;

namespace MusicManagementDemo.Application.UseCase.Music.ReadAllMusicList;

internal sealed class ReadAllMusicListQueryHandler(IMusicAppDbContext dbContext)
    : IRequestHandler<ReadAllMusicListQuery, IServiceResult>
{
    public async Task<IServiceResult> Handle(
        ReadAllMusicListQuery request,
        CancellationToken cancellationToken
    )
    {
        var musicListsToReadQuery = dbContext.MusicList.AsQueryable();
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
            .ToArrayAsync(cancellationToken: cancellationToken);
        return ServiceResult.Ok(musicListsToRead);
    }
}
