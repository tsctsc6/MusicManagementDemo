using MediatR;
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
        var createMusicListResult = await mediator.Send(command, cancellationToken);
        return Guid.Parse(createMusicListResult.Data!.GetPropertyValue("Id")!.ToString()!);
    }

    public static async Task AddMusicInfoToMusicListAsync(
        IMediator mediator,
        AddMusicInfoToMusicListCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var createMusicListResult = await mediator.Send(command, cancellationToken);
    }

    public static async Task<Guid[]> ReadAllMusicInfoAsync(
        IMediator mediator,
        ReadAllMusicInfoQuery query,
        CancellationToken cancellationToken = default
    )
    {
        var result = await mediator.Send(query, cancellationToken);
        return
        [
            .. (result.Data! as IEnumerable<object>)!.Select(o =>
                Guid.Parse(o.GetPropertyValue("Id")!.ToString()!)
            ),
        ];
    }
}
