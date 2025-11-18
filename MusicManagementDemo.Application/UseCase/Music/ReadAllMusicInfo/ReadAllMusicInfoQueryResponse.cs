using System.Collections;

namespace MusicManagementDemo.Application.UseCase.Music.ReadAllMusicInfo;

public sealed record ReadAllMusicInfoQueryResponse
    : IReadOnlyCollection<ReadAllMusicInfoQueryResponseItem>
{
    private readonly ReadAllMusicInfoQueryResponseItem[] items;

    public ReadAllMusicInfoQueryResponse(ReadAllMusicInfoQueryResponseItem[] items)
    {
        this.items = items;
    }

    public IEnumerator<ReadAllMusicInfoQueryResponseItem> GetEnumerator()
    {
        return ((IEnumerable<ReadAllMusicInfoQueryResponseItem>)items).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public int Count => items.Length;
}

public sealed record ReadAllMusicInfoQueryResponseItem(
    Guid Id,
    string Title,
    string Artist,
    string Album
);
