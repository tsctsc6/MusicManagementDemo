using System.Collections;

namespace MusicManagementDemo.Application.UseCase.Management.ReadAllStorage;

public sealed record ReadAllStorageQueryResponse
    : IReadOnlyCollection<ReadAllStorageQueryResponseItem>
{
    private readonly ReadAllStorageQueryResponseItem[] items;

    public ReadAllStorageQueryResponse(ReadAllStorageQueryResponseItem[] items)
    {
        this.items = items;
    }

    public IEnumerator<ReadAllStorageQueryResponseItem> GetEnumerator()
    {
        return ((IEnumerable<ReadAllStorageQueryResponseItem>)items).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public int Count => items.Length;
}

public sealed record ReadAllStorageQueryResponseItem(int Id, string Name, string Path);
