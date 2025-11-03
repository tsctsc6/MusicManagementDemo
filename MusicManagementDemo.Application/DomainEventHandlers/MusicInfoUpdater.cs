using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MusicManagementDemo.Abstractions.IDbContext;
using MusicManagementDemo.Domain.DomainEvents;
using MusicManagementDemo.Domain.Entity.Music;

namespace MusicManagementDemo.Application.DomainEventHandlers;

internal sealed class MusicInfoUpdater(
    IMusicAppDbContext dbContext,
    ILogger<MusicInfoUpdater> logger
) : INotificationHandler<MusicFileFoundEvent>
{
    public async ValueTask Handle(
        MusicFileFoundEvent notification,
        CancellationToken cancellationToken
    )
    {
        var newMusicInfo = new List<MusicInfo>(notification.Items.Count());
        foreach (var item in notification.Items)
        {
            var oldMusicInfo = await dbContext
                .MusicInfos.Where(e =>
                    e.Title == item.Title && e.Artist == item.Artist && e.Album == item.Album
                )
                .SingleOrDefaultAsync(cancellationToken: cancellationToken);
            if (oldMusicInfo is null)
            {
                newMusicInfo.Add(
                    new()
                    {
                        Title = item.Title,
                        Artist = item.Artist,
                        Album = item.Album,
                        FilePath = item.FilePath,
                        StorageId = item.StorageId,
                    }
                );
            }
            else
            {
                oldMusicInfo.FilePath = item.FilePath;
            }
        }
        await dbContext.MusicInfos.AddRangeAsync(newMusicInfo, cancellationToken);
        var submitCount = await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("{submitCount} rows Updated", submitCount);
    }
}
