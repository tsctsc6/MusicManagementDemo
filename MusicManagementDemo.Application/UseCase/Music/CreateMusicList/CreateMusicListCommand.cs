using MediatR;
using MusicManagementDemo.Abstractions;

namespace MusicManagementDemo.Application.UseCase.Music.CreateMusicList;

public sealed record CreateMusicListCommand(string UserId, string Name) : IRequest<IServiceResult>;
