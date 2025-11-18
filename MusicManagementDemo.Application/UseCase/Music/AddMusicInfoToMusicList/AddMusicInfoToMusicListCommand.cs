using Mediator;
using MusicManagementDemo.Application.Responses;

namespace MusicManagementDemo.Application.UseCase.Music.AddMusicInfoToMusicList;

public sealed record AddMusicInfoToMusicListCommand(Guid UserId, Guid MusicListId, Guid MusicInfoId)
    : IRequest<ApiResult<AddMusicInfoToMusicListCommandResponse>>;
