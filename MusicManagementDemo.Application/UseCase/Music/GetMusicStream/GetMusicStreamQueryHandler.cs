using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MusicManagementDemo.Abstractions.IDbContext;
using RustSharp;

namespace MusicManagementDemo.Application.UseCase.Music.GetMusicStream;

public class GetMusicStreamQueryHandler(
    IMusicAppDbContext musicDbContext,
    IManagementAppDbContext managementDbContext,
    ILogger<GetMusicStreamQueryHandler> logger
) : IRequestHandler<GetMusicStreamQuery, Option<Stream>>
{
    public async Task<Option<Stream>> Handle(
        GetMusicStreamQuery request,
        CancellationToken cancellationToken
    )
    {
        var musicInfoToGetStream = await musicDbContext
            .MusicInfo.AsNoTracking()
            .SingleOrDefaultAsync(
                e => e.Id == request.MusicInfoId,
                cancellationToken: cancellationToken
            );
        if (musicInfoToGetStream is null)
        {
            logger.LogError("MusicInfo {RequestMusicInfoId} not found", request.MusicInfoId);
            return Option<Stream>.None();
        }

        var storageToRead = await managementDbContext
            .Storage.AsNoTracking()
            .SingleOrDefaultAsync(
                e => e.Id == musicInfoToGetStream.StorageId,
                cancellationToken: cancellationToken
            );
        if (storageToRead is null)
        {
            logger.LogError("Storage {StorageId} not found", musicInfoToGetStream.StorageId);
            return Option<Stream>.None();
        }

        string fullPath = Path.Combine(storageToRead.Path, musicInfoToGetStream.FilePath);
        Stream stream;
        try
        {
            stream = File.OpenRead(fullPath);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Can not open file: {FilePath}", fullPath);
            return Option<Stream>.None();
        }
        return Option<Stream>.Some(stream);
    }
}
