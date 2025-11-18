using Mediator;
using MusicManagementDemo.Application.Responses;

namespace MusicManagementDemo.Application.UseCase.Music.ChangeMusicInfoOrderInMusicList;

public sealed record ChangeMusicInfoOrderInMusicListCommand(
    Guid UserId,
    Guid MusicListId,
    Guid TargetMusicInfoId,
    Guid? PrevMusicInfoId,
    Guid? NextMusicInfoId
) : IRequest<ApiResult<ChangeMusicInfoOrderInMusicListCommandResponse>>;
