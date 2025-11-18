using Mediator;
using MusicManagementDemo.Application.UseCase.Music.AddMusicInfoToMusicList;
using MusicManagementDemo.Application.UseCase.Music.CreateMusicList;
using MusicManagementDemo.Application.UseCase.Music.ReadAllMusicInfo;

namespace FunctionalTesting.Provisions;

public static class MusicProvision
{
    public static async Task<Guid> CreateMusicListAsync(
        IMediator mediator,
        CreateMusicListCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.Data!.Id;
    }

    public static async Task AddMusicInfoToMusicListAsync(
        IMediator mediator,
        AddMusicInfoToMusicListCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var result = await mediator.Send(command, cancellationToken);
    }

    public static async Task<Guid[]> ReadAllMusicInfoAsync(
        IMediator mediator,
        ReadAllMusicInfoQuery query,
        CancellationToken cancellationToken = default
    )
    {
        var result = await mediator.Send(query, cancellationToken);
        return [.. result.Data!.Select(e => e.Id)];
    }
}
