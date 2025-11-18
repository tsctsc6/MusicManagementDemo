namespace MusicManagementDemo.Application.UseCase.Music.GetAllMusicInfoFromMusicList;

public sealed record GetAllMusicInfoFromMusicListQueryResponse(
    string MusicListName,
    IReadOnlyCollection<GetAllMusicInfoFromMusicListQueryResponseMusicInfo> MusicInfo
);

public sealed record GetAllMusicInfoFromMusicListQueryResponseMusicInfo(
    Guid Id,
    string Title,
    string Artist,
    string Album
);
