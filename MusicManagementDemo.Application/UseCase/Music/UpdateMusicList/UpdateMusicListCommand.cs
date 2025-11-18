using Mediator;
using MusicManagementDemo.Application.Responses;

namespace MusicManagementDemo.Application.UseCase.Music.UpdateMusicList;

public sealed record UpdateMusicListCommand(Guid UserId, Guid MusicListId, string Name)
    : IRequest<ApiResult<UpdateMusicListCommandResponse>>;
