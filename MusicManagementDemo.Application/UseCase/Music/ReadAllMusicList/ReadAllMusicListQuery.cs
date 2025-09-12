using MediatR;
using MusicManagementDemo.Abstractions;

namespace MusicManagementDemo.Application.UseCase.Music.ReadAllMusicList;

public sealed record ReadAllMusicListQuery(string UserId, Guid ReferenceId, int PageSize, bool Asc)
    : IRequest<IServiceResult>;
