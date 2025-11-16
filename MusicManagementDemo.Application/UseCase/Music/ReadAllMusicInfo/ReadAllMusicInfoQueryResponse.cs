namespace MusicManagementDemo.Application.UseCase.Music.ReadAllMusicInfo;

public sealed record ReadAllMusicInfoQueryResponse(
    Guid Id,
    string Title,
    string Artist,
    string Album
);
