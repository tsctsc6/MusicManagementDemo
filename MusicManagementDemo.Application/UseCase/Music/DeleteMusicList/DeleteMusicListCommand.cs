using MediatR;
using MusicManagementDemo.Abstractions;

namespace MusicManagementDemo.Application.UseCase.Music.DeleteMusicList;

public sealed record DeleteMusicListCommand(string UserId, Guid MusicListId) : IRequest<IServiceResult>;
