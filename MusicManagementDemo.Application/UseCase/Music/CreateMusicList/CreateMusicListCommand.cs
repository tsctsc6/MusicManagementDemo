using Mediator;
using MusicManagementDemo.Application.Responses;

namespace MusicManagementDemo.Application.UseCase.Music.CreateMusicList;

public sealed record CreateMusicListCommand(Guid UserId, string Name)
    : IRequest<ApiResult<CreateMusicListCommandResponse>>;
