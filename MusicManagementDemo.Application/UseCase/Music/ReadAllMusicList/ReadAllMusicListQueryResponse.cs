namespace MusicManagementDemo.Application.UseCase.Music.ReadAllMusicList;

public sealed record ReadAllMusicListQueryResponse(Guid Id, string Name, DateTime CreatedAt);
