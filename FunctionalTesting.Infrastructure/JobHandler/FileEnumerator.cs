using MusicManagementDemo.Abstractions;

namespace FunctionalTesting.Infrastructure.JobHandler;

public class FileEnumerator : IFileEnumerator
{
    public IEnumerable<string> EnumerateFiles(
        DirectoryInfo rootDir,
        string searchPattern,
        SearchOption searchOption
    )
    {
        throw new NotImplementedException();
    }
}
