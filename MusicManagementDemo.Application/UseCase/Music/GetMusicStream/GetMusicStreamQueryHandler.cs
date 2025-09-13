using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MusicManagementDemo.Abstractions.IDbContext;
using RustSharp;

namespace MusicManagementDemo.Application.UseCase.Music.GetMusicStream;

public class GetMusicStreamQueryHandler(
    IMusicAppDbContext dbContext,
    ILogger<GetMusicStreamQueryHandler> logger
) : IRequestHandler<GetMusicStreamQuery, Option<Stream>>
{
    public async Task<Option<Stream>> Handle(
        GetMusicStreamQuery request,
        CancellationToken cancellationToken
    )
    {
        var musicInfoToGetStream = await dbContext.MusicInfo.AsNoTracking().SingleOrDefaultAsync(
            e => e.Id == request.MusicInfoId,
            cancellationToken: cancellationToken
        );
        if (musicInfoToGetStream is null)
        {
            logger.LogError("MusicInfo with id {RequestMusicInfoId} not found", request.MusicInfoId);
            return Option<Stream>.None();
        }

        Stream stream;
        try
        {
            stream = File.OpenRead(musicInfoToGetStream.FilePath);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Can not open file: {FilePath}", musicInfoToGetStream.FilePath);
            return Option<Stream>.None();
        }
        return Option<Stream>.Some(stream);
    }
}
