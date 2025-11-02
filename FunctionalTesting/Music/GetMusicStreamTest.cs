using System.Text.Json.Nodes;
using MusicManagementDemo.Application.UseCase.Management.CreateJob;
using MusicManagementDemo.Application.UseCase.Management.CreateStorage;
using MusicManagementDemo.Application.UseCase.Music.GetMusicStream;
using MusicManagementDemo.Application.UseCase.Music.ReadAllMusicInfo;
using MusicManagementDemo.Domain.Entity.Management;

namespace FunctionalTesting.Music;

public class GetMusicStreamTest : BaseTestingClass
{
    private Guid[] musicInfoIds = [];

    private async Task PrepareAsync()
    {
        var createStorageResult = await Mediator.Send(
            new CreateStorageCommand("Test", "X:\\storage1"),
            TestContext.Current.CancellationToken
        );
        var storageId = (int)createStorageResult.Data!.GetPropertyValue("Id")!;
        var createJobResult = await Mediator.Send(
            new CreateJobCommand(
                JobType.ScanIncremental,
                "ddd",
                JsonNode.Parse($"{{\"storageId\": {storageId}}}")!
            ),
            TestContext.Current.CancellationToken
        );
        var jobId = (long)createJobResult.Data?.GetPropertyValue("JobId")!;
        await Task.Delay(TimeSpan.FromSeconds(6), TestContext.Current.CancellationToken);

        var readAllMusicInfoResult = await Mediator.Send(
            new ReadAllMusicInfoQuery(null, 10, false, string.Empty),
            TestContext.Current.CancellationToken
        );
        musicInfoIds =
        [
            .. (readAllMusicInfoResult.Data! as IEnumerable<object>)!.Select(o =>
                Guid.Parse(o.GetPropertyValue("Id")!.ToString()!)
            ),
        ];
    }

    [Fact]
    public async Task Normal()
    {
        await PrepareAsync();
        var result = await Mediator.Send(
            new GetMusicStreamQuery(musicInfoIds[0]),
            TestContext.Current.CancellationToken
        );
        Assert.True(!result.IsNone);
    }

    [Fact]
    public async Task NotExist()
    {
        await PrepareAsync();
        var result = await Mediator.Send(
            new GetMusicStreamQuery(Guid.Empty),
            TestContext.Current.CancellationToken
        );
        Assert.True(result.IsNone);
    }
}
