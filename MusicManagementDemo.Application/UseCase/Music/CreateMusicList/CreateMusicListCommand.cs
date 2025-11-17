using Mediator;

namespace MusicManagementDemo.Application.UseCase.Music.CreateMusicList;

public sealed record CreateMusicListCommand(Guid UserId, string Name) : IRequest<IServiceResult>;
