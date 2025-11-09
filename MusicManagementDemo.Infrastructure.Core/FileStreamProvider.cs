using MusicManagementDemo.Abstractions;
using RustSharp;

namespace MusicManagementDemo.Infrastructure.Core;

internal class FileStreamProvider : IFileStreamProvider
{
    public Result<Stream, string> OpenRead(string filePath)
    {
        try
        {
            return Result<Stream, string>.Ok(File.OpenRead(filePath));
        }
        catch (Exception e)
        {
            return Result.Err(e.Message);
        }
    }
}
