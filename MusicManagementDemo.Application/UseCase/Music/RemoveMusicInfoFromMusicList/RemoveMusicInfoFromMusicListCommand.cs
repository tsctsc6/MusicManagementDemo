using MediatR;
using MusicManagementDemo.Abstractions;

namespace MusicManagementDemo.Application.UseCase.Music.RemoveMusicInfoFromMusicList;

public sealed record RemoveMusicInfoFromMusicListCommand(
    string UserId,
    Guid MusicListId,
    Guid MusicInfoId
) : IRequest<IServiceResult>;
