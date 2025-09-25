using Bogus;
using MusicManagementDemo.Abstractions;

namespace FunctionalTesting.Infrastructure.JobHandler;

internal sealed class FileEnumerator : IFileEnumerator
{
    public IEnumerable<string> EnumerateFiles(
        DirectoryInfo rootDir,
        string searchPattern,
        SearchOption searchOption
    )
    {
        var storage = VirtualFileSystem.VirtualFileSystem.VirtualStorages.SingleOrDefault(s =>
            s.Path == rootDir.FullName
        );
        if (storage is null)
        {
            yield break;
        }
        foreach (var file in storage.Files)
        {
            yield return Path.Combine(rootDir.FullName, file.Path);
        }
    }
}
