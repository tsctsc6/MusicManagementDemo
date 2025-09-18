using MusicManagementDemo.Abstractions;

namespace MusicManagementDemo.Infrastructure.JobHandler;

internal sealed class FileEnumerator : IFileEnumerator
{
    public IEnumerable<string> EnumerateFiles(
        DirectoryInfo rootDir,
        string searchPattern,
        SearchOption searchOption
    )
    {
        return rootDir
            .EnumerateFiles(searchPattern, searchOption)
            .Select(fileInfo => fileInfo.FullName);
    }
}
