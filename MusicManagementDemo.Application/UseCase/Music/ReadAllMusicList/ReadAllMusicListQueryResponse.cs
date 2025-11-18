using System.Collections;

namespace MusicManagementDemo.Application.UseCase.Music.ReadAllMusicList;

public sealed record ReadAllMusicListQueryResponse
    : IReadOnlyCollection<ReadAllMusicListQueryResponseItem>
{
    private readonly ReadAllMusicListQueryResponseItem[] items;

    public ReadAllMusicListQueryResponse(ReadAllMusicListQueryResponseItem[] items)
    {
        this.items = items;
    }

    public IEnumerator<ReadAllMusicListQueryResponseItem> GetEnumerator()
    {
        return ((IEnumerable<ReadAllMusicListQueryResponseItem>)items).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public int Count => items.Length;
}

public sealed record ReadAllMusicListQueryResponseItem(Guid Id, string Name, DateTime CreatedAt);
