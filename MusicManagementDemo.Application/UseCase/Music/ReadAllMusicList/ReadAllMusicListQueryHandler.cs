using MediatR;
using Microsoft.EntityFrameworkCore;
using MusicManagementDemo.Abstractions;
using MusicManagementDemo.Abstractions.IDbContext;
using MusicManagementDemo.Application.Responses;
using MusicManagementDemo.Domain.Entity.Music;

namespace MusicManagementDemo.Application.UseCase.Music.ReadAllMusicList;

internal sealed class ReadAllMusicListQueryHandler(IMusicAppDbContext dbContext)
    : IRequestHandler<ReadAllMusicListQuery, IServiceResult>
{
    public async Task<IServiceResult> Handle(
        ReadAllMusicListQuery request,
        CancellationToken cancellationToken
    )
    {
        MusicList[] musicListsToRead = [];
        if (request.Asc)
        {
            musicListsToRead = await dbContext
            .MusicList.OrderBy(e => e.Id)
            .Where(e => e.UserId == request.UserId && e.Id > request.ReferenceId)
            .Take(request.PageSize)
            .AsNoTracking()
            .ToArrayAsync(cancellationToken: cancellationToken);
        }
        else
        {
            musicListsToRead = await dbContext
            .MusicList.OrderByDescending(e => e.Id)
            .Where(e => e.UserId == request.UserId && e.Id < request.ReferenceId)
            .Take(request.PageSize)
            .AsNoTracking()
            .ToArrayAsync(cancellationToken: cancellationToken);
        }
        return ServiceResult.Ok(musicListsToRead);
    }
}
