using MediatR;
using MusicManagementDemo.Abstractions;

namespace MusicManagementDemo.Application.UseCase.Music.ChangeMusicInfoOrderInMusicList;

public sealed record ChangeMusicInfoOrderInMusicListCommand(
    string UserId,
    Guid MusicListId,
    Guid TargetMusicInfoId,
    Guid? PrevMusicInfoId,
    Guid? NextMusicInfoId
) : IRequest<IServiceResult>;
