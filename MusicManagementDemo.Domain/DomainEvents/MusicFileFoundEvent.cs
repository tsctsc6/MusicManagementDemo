using MediatR;

namespace MusicManagementDemo.Domain.DomainEvents;

public sealed record MusicFileFoundEvent(IEnumerable<MusicFileFoundEventItem> Items)
    : INotification;

public sealed record MusicFileFoundEventItem(
    string Title,
    string Artist,
    string Album,
    string FilePath,
    int StorageId
);
