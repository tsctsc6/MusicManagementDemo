using Mediator;
using MusicManagementDemo.Application.Responses;

namespace MusicManagementDemo.Application.UseCase.Music.GetAllMusicInfoFromMusicList;

public sealed record GetAllMusicInfoFromMusicListQuery(
    Guid UserId,
    Guid MusicListId,
    int PageSize,
    Guid? ReferenceId,
    bool Asc
) : IRequest<ApiResult<GetAllMusicInfoFromMusicListQueryResponse>>;
