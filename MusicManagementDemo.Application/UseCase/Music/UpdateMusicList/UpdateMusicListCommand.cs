using MediatR;
using MusicManagementDemo.Abstractions;

namespace MusicManagementDemo.Application.UseCase.Music.UpdateMusicList;

public sealed record UpdateMusicListCommand(string UserId, Guid MusicListId, string Name)
    : IRequest<IServiceResult>;
