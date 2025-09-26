using MusicManagementDemo.Abstractions;
using RustSharp;

namespace FunctionalTesting.Infrastructure.JobHandler;

internal sealed class FileEnumerator : IFileEnumerator
{
    public Result<IEnumerable<string>, string> EnumerateFiles(
        string rootDir,
        string searchPattern,
        SearchOption searchOption
    )
    {
        var storage = VirtualFileSystem.VirtualFileSystem.VirtualStorages.SingleOrDefault(s =>
            s.Path == rootDir
        );
        if (storage is null)
        {
            return Result.Err($"storage.Path {rootDir} not found");
        }
        return Result.Ok(storage.Files.Select(f => Path.Combine(rootDir, f.Path)));
    }
}
