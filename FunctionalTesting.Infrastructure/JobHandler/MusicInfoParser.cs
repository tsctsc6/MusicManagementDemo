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
        var relativePath = Path.GetRelativePath(storagePath, fullPath);
        var array = relativePath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        return Task.FromResult<Result<MusicFileFoundEventItem, string>>(
            Result.Ok(
                new MusicFileFoundEventItem(
                    Title: array[2],
                    Artist: array[0],
                    Album: array[1],
                    FilePath: relativePath,
                    StorageId: storageId
                )
            )
        );
    }
}
