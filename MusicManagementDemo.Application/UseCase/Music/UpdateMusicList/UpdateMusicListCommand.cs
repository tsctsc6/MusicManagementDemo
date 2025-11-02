using Mediator;
using MusicManagementDemo.Abstractions;

namespace MusicManagementDemo.Application.UseCase.Music.UpdateMusicList;

public sealed record UpdateMusicListCommand(Guid UserId, Guid MusicListId, string Name)
    : IRequest<IServiceResult>;
