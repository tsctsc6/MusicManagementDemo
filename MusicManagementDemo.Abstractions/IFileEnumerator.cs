using RustSharp;

namespace MusicManagementDemo.Abstractions;

public interface IFileEnumerator
{
    /// <summary>
    /// Returns an enumerable collection of file information that matches a specified search pattern and search subdirectory option.
    /// </summary>
    /// <param name="rootDir">The directory to enumerate files.</param>
    /// <param name="searchPattern">The search string to match against the names of files. This parameter can contain a combination of valid literal path and wildcard (* and ?) characters, but it doesn't support regular expressions.</param>
    /// <param name="searchOption">One of the enumeration values that specifies whether the search operation should include only the current directory or all subdirectories. The default value is TopDirectoryOnly.</param>
    /// <returns>An enumerable collection of full path of files that matches searchPattern and searchOption.</returns>
    public Result<IEnumerable<string>, string> EnumerateFiles(
        string rootDir,
        string searchPattern,
        SearchOption searchOption
    );
}
