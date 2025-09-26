using MusicManagementDemo.Abstractions;
using MusicManagementDemo.Domain.DomainEvents;
using RustSharp;

namespace FunctionalTesting.Infrastructure.JobHandler;

public class MusicInfoParser : IMusicInfoParser
{
    public async Task<Result<MusicFileFoundEventItem, string>> ParseMusicInfoAsync(
        string fullPath,
        int storageId,
        string storagePath,
        CancellationToken cancellationToken = default
    )
    {
        await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
        var storage = VirtualFileSystem.VirtualFileSystem.VirtualStorages.SingleOrDefault(s =>
            s.Path == storagePath
        );
        if (storage is null)
        {
            return Result.Err("storage not found");
        }
        var relativePath = Path.GetRelativePath(storagePath, fullPath);
        var file = storage.Files.SingleOrDefault(f => f.Path == relativePath);
        if (file is null)
        {
            return Result.Err("file not found");
        }
        return Result.Ok(
            new MusicFileFoundEventItem(
                Title: file.Title,
                Artist: file.Artist,
                Album: file.Album,
                FilePath: relativePath,
                StorageId: storageId
            )
        );
    }
}
