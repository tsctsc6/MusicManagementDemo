using MediatR;
using MusicManagementDemo.Application.Responses;

namespace MusicManagementDemo.Application.UseCase.Music.CreateMusicList;

public sealed record CreateMusicListCommand(string UserId, string Name) : IRequest<ServiceResult>;
