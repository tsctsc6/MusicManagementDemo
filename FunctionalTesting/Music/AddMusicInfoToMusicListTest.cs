using System.Text.Json.Nodes;
using MusicManagementDemo.Application.UseCase.Identity.Register;
using MusicManagementDemo.Application.UseCase.Management.CreateJob;
using MusicManagementDemo.Application.UseCase.Management.CreateStorage;
using MusicManagementDemo.Application.UseCase.Music.AddMusicInfoToMusicList;
using MusicManagementDemo.Application.UseCase.Music.CreateMusicList;
using MusicManagementDemo.Application.UseCase.Music.ReadAllMusicInfo;
using MusicManagementDemo.Domain.Entity.Management;

namespace FunctionalTesting.Music;

public class AddMusicInfoToMusicListTest : BaseTestingClass
{
    private Guid userId;
    private Guid musicListId;
    private Guid[] musicInfoIds;

    private async Task PrepareAsync()
    {
        var regResult = await mediator.Send(
            new RegisterCommand(Email: "aaa@aaa.com", UserName: "aaa", Password: "Abc@123"),
            TestContext.Current.CancellationToken
        );
        userId = Guid.Parse(regResult.Data!.ToString()!);
        var createMusicListResult = await mediator.Send(
            new CreateMusicListCommand(userId, "New MusicList"),
            TestContext.Current.CancellationToken
        );
        musicListId = Guid.Parse(createMusicListResult.Data!.GetProperty("Id")!.ToString()!);

        var createStorageResult = await mediator.Send(
            new CreateStorageCommand("Test", "X:\\storage1"),
            TestContext.Current.CancellationToken
        );
        var storageId = (int)createStorageResult.Data!;
        var createJobResult = await mediator.Send(
            new CreateJobCommand(
                JobType.ScanIncremental,
                "ddd",
                JsonNode.Parse($"{{\"storageId\": {storageId}}}")!
            ),
            TestContext.Current.CancellationToken
        );
        var jobId = (long)createJobResult.Data?.GetProperty("JobId")!;
        await Task.Delay(TimeSpan.FromSeconds(6), TestContext.Current.CancellationToken);

        var readAllMusicInfoResult = await mediator.Send(
            new ReadAllMusicInfoQuery(null, 10, false, string.Empty),
            TestContext.Current.CancellationToken
        );
        musicInfoIds =
        [
            .. (readAllMusicInfoResult.Data! as IEnumerable<object>)!.Select(o =>
                Guid.Parse(o.GetProperty("Id")!.ToString()!)
            ),
        ];
    }

    [Fact]
    public async Task Normal()
    {
        await PrepareAsync();
        var result = await mediator.Send(
            new AddMusicInfoToMusicListCommand(userId, musicListId, musicInfoIds[0]),
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
    }

    [Fact]
    public async Task Repeatedly()
    {
        await PrepareAsync();
        await mediator.Send(
            new AddMusicInfoToMusicListCommand(userId, musicListId, musicInfoIds[0]),
            TestContext.Current.CancellationToken
        );
        var result = await mediator.Send(
            new AddMusicInfoToMusicListCommand(userId, musicListId, musicInfoIds[0]),
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(result);
        Assert.NotEqual(200, result.Code);
    }
}
