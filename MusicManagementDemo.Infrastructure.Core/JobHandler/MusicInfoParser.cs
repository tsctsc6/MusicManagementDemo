using System.Diagnostics;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using MusicManagementDemo.Abstractions;
using MusicManagementDemo.Domain.DomainEvents;
using RustSharp;

namespace MusicManagementDemo.Infrastructure.Core.JobHandler;

internal sealed class MusicInfoParser(ILogger<MusicInfoParser> logger) : IMusicInfoParser
{
    public async Task<Result<MusicFileFoundEventItem, string>> ParseMusicInfoAsync(
        string fullPath,
        int storageId,
        string storagePath,
        CancellationToken cancellationToken = default
    )
    {
        using var ffprobeProcess = new Process();
        ffprobeProcess.StartInfo = new ProcessStartInfo
        {
            FileName = "ffprobe",
            Arguments = $"""-v error -i "{fullPath}" -print_format json -show_format""",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
        };
        ffprobeProcess.Start();
        await ffprobeProcess.WaitForExitAsync(cancellationToken);
        var result = await ffprobeProcess.StandardOutput.ReadToEndAsync(cancellationToken);
        ffprobeProcess.Close();

        logger.LogInformation("ffprobe result: {result}", result);

        var resultJsonNode = JsonNode.Parse(result);
        if (resultJsonNode is null)
        {
            logger.LogError("Can't parse ffprobe output to JsonNode");
            return Result.Err("Can't parse ffprobe output to JsonNode");
        }
        var resultFormatJsonObject = resultJsonNode["format"]?.AsObject();
        if (resultFormatJsonObject is null)
        {
            logger.LogError("Can't find \"format\" in ffprobe output");
            return Result.Err("Can't find \"format\" in ffprobe output");
        }
        var resultFormatTagsJsonObject = resultFormatJsonObject["tags"]?.AsObject();
        if (resultFormatTagsJsonObject is null)
        {
            logger.LogError("Can't find \"format:tags\" in ffprobe output");
            return Result.Err("Can't find \"format:tags\" in ffprobe output");
        }

        var title = resultFormatTagsJsonObject["title"]?.GetValue<string>() ?? string.Empty;
        if (string.IsNullOrEmpty(title))
        {
            logger.LogError("Can't find \"format:tags:title\" in ffprobe output");
            return Result.Err("Can't find \"format:tags:title\" in ffprobe output");
        }
        var artist = resultFormatTagsJsonObject["artist"]?.GetValue<string>() ?? string.Empty;
        if (string.IsNullOrEmpty(artist))
        {
            logger.LogError("Can't find \"format:tags:artist\" in ffprobe output");
            return Result.Err("Can't find \"format:tags:artist\" in ffprobe output");
        }
        var album = resultFormatTagsJsonObject["album"]?.GetValue<string>() ?? string.Empty;
        if (string.IsNullOrEmpty(album))
        {
            logger.LogError("Can't find \"format:tags:album\" in ffprobe output");
            return Result.Err("Can't find \"format:tags:album\" in ffprobe output");
        }

        return Result.Ok(
            new MusicFileFoundEventItem(
                Title: title,
                Artist: artist,
                Album: album,
                FilePath: Path.GetRelativePath(storagePath, fullPath),
                StorageId: storageId
            )
        );
    }
}
