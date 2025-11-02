using Mediator;
using MusicManagementDemo.Abstractions;

namespace MusicManagementDemo.Application.UseCase.Music.DeleteMusicList;

public sealed record DeleteMusicListCommand(Guid UserId, Guid MusicListId)
    : IRequest<IServiceResult>;
