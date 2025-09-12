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
        var musicListsToRead = await dbContext
            .MusicList.OrderByDescending(e => e.Id)
            .Where(e => e.UserId == request.UserId && e.Id < request.ReferenceId)
            .Take(request.PageSize)
            .ToArrayAsync(cancellationToken: cancellationToken);
        return ServiceResult.Ok(musicListsToRead);
    }
}
