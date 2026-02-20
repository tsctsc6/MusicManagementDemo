using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MusicManagementDemo.Abstractions.IDbContext;
using MusicManagementDemo.Application.Responses;
using MusicManagementDemo.Domain.Entity.Music;
using static MusicManagementDemo.Application.Responses.ApiResult<MusicManagementDemo.Application.UseCase.Music.ChangeMusicInfoOrderInMusicList.ChangeMusicInfoOrderInMusicListCommandResponse>;

namespace MusicManagementDemo.Application.UseCase.Music.ChangeMusicInfoOrderInMusicList;

internal sealed class ChangeMusicInfoOrderInMusicListCommandHandler(
    IMusicAppDbContext dbContext,
    ILogger<ChangeMusicInfoOrderInMusicListCommandHandler> logger
)
    : IRequestHandler<
        ChangeMusicInfoOrderInMusicListCommand,
        ApiResult<ChangeMusicInfoOrderInMusicListCommandResponse>
    >
{
    public async ValueTask<ApiResult<ChangeMusicInfoOrderInMusicListCommandResponse>> Handle(
        ChangeMusicInfoOrderInMusicListCommand request,
        CancellationToken cancellationToken
    )
    {
        return Ok(new ChangeMusicInfoOrderInMusicListCommandResponse());
    }

    /// <summary>
    /// 检查 request.PrevMusicInfoId 和 request.NextMusicInfoId 是否相邻
    /// 并且 request.PrevMusicInfoId 在前， request.NextMusicInfoId 再后
    /// 如果 request.PrevMusicInfoId is null ， 检查 request.NextMusicInfoId 是否第一个节点
    /// 如果 request.NextMusicInfoId is null ， 检查 request.PrevMusicInfoId 是否最后一个节点
    /// </summary>
    /// <param name="request"></param>
    /// <param name="prevMusicInfoMap"></param>
    /// <param name="nextMusicInfoMap"></param>
    /// <returns>true for neighboring.</returns>
    private bool CheckNeighboringAsync(
        ChangeMusicInfoOrderInMusicListCommand request,
        MusicInfoMusicListMap? prevMusicInfoMap,
        MusicInfoMusicListMap? nextMusicInfoMap
    )
    {
        return false;
    }
}
