using MediatR;
using Microsoft.EntityFrameworkCore;
using MusicManagementDemo.Abstractions;
using MusicManagementDemo.Abstractions.IDbContext;
using MusicManagementDemo.Application.Responses;

namespace MusicManagementDemo.Application.UseCase.Music.RemoveMusicInfoFromMusicList;

internal sealed class RemoveMusicInfoFromMusicListHandler(IMusicAppDbContext dbContext)
    : IRequestHandler<RemoveMusicInfoFromMusicListCommand, IServiceResult>
{
    public async Task<IServiceResult> Handle(
        RemoveMusicInfoFromMusicListCommand request,
        CancellationToken cancellationToken
    )
    {
        if (
            !await dbContext.MusicList.AnyAsync(
                e => e.Id == request.MusicListId && e.UserId == request.UserId,
                cancellationToken: cancellationToken
            )
        )
        {
            return ServiceResult.Err(404, ["MusicListId not found"]);
        }

        await using var transaction = await dbContext.Database.BeginTransactionAsync(
            cancellationToken
        );
        if (
            await dbContext
                .MusicInfoMusicListMap.Where(e =>
                    e.MusicListId == request.MusicListId && e.MusicInfoId == request.MusicInfoId
                )
                .ExecuteDeleteAsync(cancellationToken: cancellationToken) != 1
        )
        {
            await transaction.RollbackAsync(cancellationToken);
            return ServiceResult.Err(404, ["MusicInfoMusicListMap not found"]);
        }
        await transaction.CommitAsync(cancellationToken);
        return ServiceResult.Ok();
    }
}
