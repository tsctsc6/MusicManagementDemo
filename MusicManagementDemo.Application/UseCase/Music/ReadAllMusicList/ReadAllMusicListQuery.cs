using Mediator;
using MusicManagementDemo.Application.Responses;

namespace MusicManagementDemo.Application.UseCase.Music.ReadAllMusicList;

public sealed record ReadAllMusicListQuery(Guid UserId, Guid? ReferenceId, int PageSize, bool Asc)
    : IRequest<ApiResult<ReadAllMusicListQueryResponse>>;
