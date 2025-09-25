using RustSharp;

namespace MusicManagementDemo.Abstractions;

public interface IFileStreamProvider
{
    public Result<Stream, string> OpenRead(string filePath);
}
