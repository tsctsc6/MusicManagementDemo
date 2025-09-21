using MusicManagementDemo.Domain.DomainEvents;
using RustSharp;

namespace MusicManagementDemo.Abstractions;

public interface IMusicInfoParser
{
    public Task<Result<MusicFileFoundEventItem, string>> ParseMusicInfoAsync(
        string fullPath,
        int storageId,
        string storagePath,
        CancellationToken cancellationToken = default
    );
}
