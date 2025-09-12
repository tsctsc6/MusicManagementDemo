using MediatR;
using MusicManagementDemo.Abstractions;

namespace MusicManagementDemo.Application.UseCase.Music.AddMusicInfoToMusicList;

public sealed record AddMusicInfoToMusicListCommand(
    string UserId,
    Guid MusicListId,
    Guid MusicInfoId
) : IRequest<IServiceResult>;
