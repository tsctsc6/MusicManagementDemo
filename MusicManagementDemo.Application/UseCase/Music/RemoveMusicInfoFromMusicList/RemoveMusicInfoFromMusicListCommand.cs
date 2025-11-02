using Mediator;
using MusicManagementDemo.Abstractions;

namespace MusicManagementDemo.Application.UseCase.Music.RemoveMusicInfoFromMusicList;

public sealed record RemoveMusicInfoFromMusicListCommand(
    Guid UserId,
    Guid MusicListId,
    Guid MusicInfoId
) : IRequest<IServiceResult>;
