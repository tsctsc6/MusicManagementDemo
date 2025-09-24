using MediatR;
using MusicManagementDemo.Abstractions;

namespace MusicManagementDemo.Application.UseCase.Music.GetAllMusicInfoFromMusicList;

public sealed record GetAllMusicInfoFromMusicListQuery(
    Guid UserId,
    Guid MusicListId,
    int PageSize,
    Guid? ReferenceId,
    bool Asc
) : IRequest<IServiceResult>;
