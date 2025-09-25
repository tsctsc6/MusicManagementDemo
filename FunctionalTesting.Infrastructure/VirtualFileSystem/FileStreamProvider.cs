using MusicManagementDemo.Abstractions;
using RustSharp;

namespace FunctionalTesting.Infrastructure.VirtualFileSystem;

internal sealed class FileStreamProvider : IFileStreamProvider
{
    public Result<Stream, string> OpenRead(string filePath)
    {
        // flac file header
        byte[] bytes = [0x66, 0x4C, 0x61, 0x43];
        return Result<Stream, string>.Ok(new MemoryStream(bytes));
    }
}
