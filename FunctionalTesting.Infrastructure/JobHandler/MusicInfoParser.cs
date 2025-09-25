using MusicManagementDemo.Abstractions;
using MusicManagementDemo.Domain.DomainEvents;
using RustSharp;

namespace FunctionalTesting.Infrastructure.JobHandler;

public class MusicInfoParser : IMusicInfoParser
{
    public Task<Result<MusicFileFoundEventItem, string>> ParseMusicInfoAsync(
        string fullPath,
        int storageId,
        string storagePath,
        CancellationToken cancellationToken = default
    )
    {
        var storage = VirtualFileSystem.VirtualFileSystem.VirtualStorages.SingleOrDefault(s =>
            s.Path == storagePath
        );
        if (storage is null)
        {
            return Task.FromResult<Result<MusicFileFoundEventItem, string>>(
                Result.Err("storage not found")
            );
        }
        var relativePath = Path.GetRelativePath(storagePath, fullPath);
        var file = storage.Files.SingleOrDefault(f => f.Path == relativePath);
        if (file is null)
        {
            return Task.FromResult<Result<MusicFileFoundEventItem, string>>(
                Result.Err("file not found")
            );
        }
        return Task.FromResult<Result<MusicFileFoundEventItem, string>>(
            Result.Ok(
                new MusicFileFoundEventItem(
                    Title: file.Title,
                    Artist: file.Artist,
                    Album: file.Album,
                    FilePath: relativePath,
                    StorageId: storageId
                )
            )
        );
    }
}
