using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MusicManagementDemo.Abstractions.IDbContext;
using MusicManagementDemo.Application.Responses;
using MusicManagementDemo.Domain.Entity.Music;
using static MusicManagementDemo.Application.Responses.ApiResult<MusicManagementDemo.Application.UseCase.Music.AddMusicInfoToMusicList.AddMusicInfoToMusicListCommandResponse>;

namespace MusicManagementDemo.Application.UseCase.Music.AddMusicInfoToMusicList;

/// <summary>
/// 添加一首歌曲到歌单的末尾。
/// </summary>
/// <param name="dbContext"></param>
internal sealed class AddMusicInfoToMusicListCommandHandler(
    IMusicAppDbContext dbContext,
    ILogger<AddMusicInfoToMusicListCommandHandler> logger
)
    : IRequestHandler<
        AddMusicInfoToMusicListCommand,
        ApiResult<AddMusicInfoToMusicListCommandResponse>
    >
{
    public async ValueTask<ApiResult<AddMusicInfoToMusicListCommandResponse>> Handle(
        AddMusicInfoToMusicListCommand request,
        CancellationToken cancellationToken
    )
    {
        return Ok(new AddMusicInfoToMusicListCommandResponse());
    }
}
