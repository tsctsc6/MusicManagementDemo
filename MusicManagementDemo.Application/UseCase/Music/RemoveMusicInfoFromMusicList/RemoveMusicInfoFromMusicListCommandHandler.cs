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
        return Ok(new RemoveMusicInfoFromMusicListCommandResponse());
    }
}
