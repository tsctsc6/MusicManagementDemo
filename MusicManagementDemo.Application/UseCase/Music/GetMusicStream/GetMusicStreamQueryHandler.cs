using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MusicManagementDemo.Abstractions;
using MusicManagementDemo.Abstractions.IDbContext;
using RustSharp;

namespace MusicManagementDemo.Application.UseCase.Music.GetMusicStream;

internal sealed class GetMusicStreamQueryHandler(
    IMusicAppDbContext musicDbContext,
    IManagementAppDbContext managementDbContext,
    IFileStreamProvider fileStreamProvider,
    ILogger<GetMusicStreamQueryHandler> logger
) : IRequestHandler<GetMusicStreamQuery, Option<Stream>>
{
    public async ValueTask<Option<Stream>> Handle(
        GetMusicStreamQuery request,
        CancellationToken cancellationToken
    )
    {
        var musicInfoToGetStream = await musicDbContext
            .MusicInfos.AsNoTracking()
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
            .Storages.AsNoTracking()
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
        var resultStream = fileStreamProvider.OpenRead(fullPath);
        switch (resultStream)
        {
            case ErrResult<Stream, string> e:
                logger.LogError("Can not open file: {FilePath}, {e}", fullPath, e.Value);
                return Option<Stream>.None();
            case OkResult<Stream, string> stream:
                return Option<Stream>.Some(stream.Value);
            default:
                logger.LogError("{resultStream}", resultStream);
                return Option<Stream>.None();
        }
    }
}
