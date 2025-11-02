using Mediator;
using RustSharp;

namespace MusicManagementDemo.Application.UseCase.Music.GetMusicStream;

public sealed record GetMusicStreamQuery(Guid MusicInfoId) : IRequest<Option<Stream>>;
