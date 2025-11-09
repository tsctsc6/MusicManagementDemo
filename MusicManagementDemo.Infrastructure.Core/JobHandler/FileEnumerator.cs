using MusicManagementDemo.Abstractions;
using RustSharp;

namespace MusicManagementDemo.Infrastructure.Core.JobHandler;

internal sealed class FileEnumerator : IFileEnumerator
{
    public Result<IEnumerable<string>, string> EnumerateFiles(
        string rootDir,
        string searchPattern,
        SearchOption searchOption
    )
    {
        if (!Directory.Exists(rootDir))
        {
            return Result.Err($"storage.Path {rootDir} not found");
        }
        return Result.Ok(
            new DirectoryInfo(rootDir)
                .EnumerateFiles(searchPattern, searchOption)
                .Select(fileInfo => fileInfo.FullName)
        );
    }
}
