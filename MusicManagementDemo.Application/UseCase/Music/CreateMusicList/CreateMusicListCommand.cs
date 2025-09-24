using MediatR;
using MusicManagementDemo.Abstractions;

namespace MusicManagementDemo.Application.UseCase.Music.CreateMusicList;

public sealed record CreateMusicListCommand(Guid UserId, string Name) : IRequest<IServiceResult>;
