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
        throw new NotImplementedException();
    }
}
