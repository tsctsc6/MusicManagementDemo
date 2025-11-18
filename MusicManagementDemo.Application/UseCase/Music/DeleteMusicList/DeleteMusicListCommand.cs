using Mediator;
using MusicManagementDemo.Application.Responses;

namespace MusicManagementDemo.Application.UseCase.Music.DeleteMusicList;

public sealed record DeleteMusicListCommand(Guid UserId, Guid MusicListId)
    : IRequest<ApiResult<DeleteMusicListCommandResponse>>;
