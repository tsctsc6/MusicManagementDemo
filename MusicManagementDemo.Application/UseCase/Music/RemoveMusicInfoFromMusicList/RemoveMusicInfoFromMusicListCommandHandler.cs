using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MusicManagementDemo.Abstractions.IDbContext;
using MusicManagementDemo.Application.Responses;
using static MusicManagementDemo.Application.Responses.ApiResult<MusicManagementDemo.Application.UseCase.Music.RemoveMusicInfoFromMusicList.RemoveMusicInfoFromMusicListCommandResponse>;

namespace MusicManagementDemo.Application.UseCase.Music.RemoveMusicInfoFromMusicList;

internal sealed class RemoveMusicInfoFromMusicListCommandHandler(
    IMusicAppDbContext dbContext,
    ILogger<RemoveMusicInfoFromMusicListCommandHandler> logger
)
    : IRequestHandler<
        RemoveMusicInfoFromMusicListCommand,
        ApiResult<RemoveMusicInfoFromMusicListCommandResponse>
    >
{
    public async ValueTask<ApiResult<RemoveMusicInfoFromMusicListCommandResponse>> Handle(
        RemoveMusicInfoFromMusicListCommand request,
        CancellationToken cancellationToken
    )
    {
        if (
            !await dbContext.MusicLists.AnyAsync(
                e => e.Id == request.MusicListId && e.UserId == request.UserId,
                cancellationToken: cancellationToken
            )
        )
        {
            logger.LogError("MusicList {MusicListId} not found", request.MusicListId);
            return Err(404, "MusicList not found");
        }

        var submited = await dbContext
            .MusicInfoMusicListMaps.Where(e =>
                e.MusicListId == request.MusicListId && e.MusicInfoId == request.MusicInfoId
            )
            .ExecuteDeleteAsync(cancellationToken);
        if (submited != 1)
        {
            logger.LogError(
                "Delete Failed, expected submit count: 1, submit count: {submited}",
                submited
            );
            return Err(404, "Delete Failed");
        }

        logger.LogInformation(
            "MusicInfoMap {MusicListId} {MusicInfoId} Removed",
            request.MusicListId,
            request.MusicInfoId
        );
        return Ok(new RemoveMusicInfoFromMusicListCommandResponse());
    }
}
