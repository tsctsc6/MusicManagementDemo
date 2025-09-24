using Bogus;
using MusicManagementDemo.Abstractions;

namespace FunctionalTesting.Infrastructure.JobHandler;

internal sealed class MusicFile
{
    public string Title { get; set; } = string.Empty;
    public string Album { get; set; } = string.Empty;
    public string Artist { get; set; } = string.Empty;
}

public class FileEnumerator : IFileEnumerator
{
    public IEnumerable<string> EnumerateFiles(
        DirectoryInfo rootDir,
        string searchPattern,
        SearchOption searchOption
    )
    {
        var musicFiles = new Faker<MusicFile>()
            .RuleFor(x => x.Title, f => f.Name.LastName())
            .RuleFor(x => x.Album, f => f.Music.Genre())
            .RuleFor(x => x.Artist, f => f.Name.FirstName())
            .Generate(12)
            .Select(f => Path.Combine(rootDir.FullName, f.Artist, f.Album, f.Title));
        return musicFiles;
    }
}
