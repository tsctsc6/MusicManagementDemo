using Mediator;
using MusicManagementDemo.Application.Responses;

namespace MusicManagementDemo.Application.UseCase.Music.RemoveMusicInfoFromMusicList;

public sealed record RemoveMusicInfoFromMusicListCommand(
    Guid UserId,
    Guid MusicListId,
    Guid MusicInfoId
) : IRequest<ApiResult<RemoveMusicInfoFromMusicListCommandResponse>>;
