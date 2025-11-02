using System.Text.Json.Nodes;
using MusicManagementDemo.Application.UseCase.Identity.Register;
using MusicManagementDemo.Application.UseCase.Management.CreateJob;
using MusicManagementDemo.Application.UseCase.Management.CreateStorage;
using MusicManagementDemo.Application.UseCase.Music.AddMusicInfoToMusicList;
using MusicManagementDemo.Application.UseCase.Music.ChangeMusicInfoOrderInMusicList;
using MusicManagementDemo.Application.UseCase.Music.CreateMusicList;
using MusicManagementDemo.Application.UseCase.Music.ReadAllMusicInfo;
using MusicManagementDemo.Domain.Entity.Management;

namespace FunctionalTesting.Music;

public class ChangeMusicInfoOrderInMusicListTest : BaseTestingClass
{
    private Guid userId;
    private Guid musicListId;
    private Guid[] musicInfoIds = [];

    private async Task PrepareAsync()
    {
        var regResult = await Mediator.Send(
            new RegisterCommand(Email: "aaa@aaa.com", UserName: "aaa", Password: "Abc@123"),
            TestContext.Current.CancellationToken
        );
        userId = Guid.Parse(regResult.Data!.GetPropertyValue("Id")!.ToString()!);
        var createMusicListResult = await Mediator.Send(
            new CreateMusicListCommand(userId, "New MusicList"),
            TestContext.Current.CancellationToken
        );
        musicListId = Guid.Parse(createMusicListResult.Data!.GetPropertyValue("Id")!.ToString()!);

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
        for (int i = 0; i < 6; i++)
        {
            await Mediator.Send(
                new AddMusicInfoToMusicListCommand(userId, musicListId, musicInfoIds[i]),
                TestContext.Current.CancellationToken
            );
        }
    }

    public record NormalArgs(int TargetIndex, int MoveToBeforeIndex);

    public static IEnumerable<TheoryDataRow<NormalArgs>> NormalTestData =>
        [new(new(0, 6)), new(new(1, 5))];

    [Theory]
    [MemberData(nameof(NormalTestData))]
    public async Task Normal(NormalArgs args)
    {
        await PrepareAsync();
        var targetMusicInfoId = musicInfoIds[args.TargetIndex];
        Guid? prevMusicInfoId;
        Guid? nextMusicInfoId;
        if (args.MoveToBeforeIndex == 0)
        {
            prevMusicInfoId = null;
            nextMusicInfoId = musicInfoIds[args.MoveToBeforeIndex];
        }
        else if (args.MoveToBeforeIndex == musicInfoIds.Length)
        {
            prevMusicInfoId = musicInfoIds[args.MoveToBeforeIndex - 1];
            nextMusicInfoId = null;
        }
        else
        {
            prevMusicInfoId = musicInfoIds[args.MoveToBeforeIndex - 1];
            nextMusicInfoId = musicInfoIds[args.MoveToBeforeIndex];
        }
        var result = await Mediator.Send(
            new ChangeMusicInfoOrderInMusicListCommand(
                userId,
                musicListId,
                targetMusicInfoId,
                prevMusicInfoId,
                nextMusicInfoId
            ),
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public async Task MusicInfoNotExist(int whichEmpty)
    {
        await PrepareAsync();
        var targetMusicInfoId = musicInfoIds[1];
        var prevMusicInfoId = musicInfoIds[2];
        var nextMusicInfoId = musicInfoIds[3];
        switch (whichEmpty)
        {
            case 0:
                targetMusicInfoId = Guid.Empty;
                break;
            case 1:
                prevMusicInfoId = Guid.Empty;
                break;
            case 2:
                nextMusicInfoId = Guid.Empty;
                break;
            default:
                Assert.Fail();
                break;
        }
        var result = await Mediator.Send(
            new ChangeMusicInfoOrderInMusicListCommand(
                userId,
                musicListId,
                targetMusicInfoId,
                prevMusicInfoId,
                nextMusicInfoId
            ),
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(result);
        Assert.NotEqual(200, result.Code);
    }

    [Fact]
    public async Task MusicListNotExist()
    {
        await PrepareAsync();
        var targetMusicInfoId = musicInfoIds[1];
        var prevMusicInfoId = musicInfoIds[2];
        var nextMusicInfoId = musicInfoIds[3];
        var result = await Mediator.Send(
            new ChangeMusicInfoOrderInMusicListCommand(
                userId,
                Guid.Empty,
                targetMusicInfoId,
                prevMusicInfoId,
                nextMusicInfoId
            ),
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(result);
        Assert.NotEqual(200, result.Code);
    }

    [Fact]
    public async Task NotNeighboring()
    {
        await PrepareAsync();
        var targetMusicInfoId = musicInfoIds[1];
        var prevMusicInfoId = musicInfoIds[2];
        var nextMusicInfoId = musicInfoIds[4];
        var result = await Mediator.Send(
            new ChangeMusicInfoOrderInMusicListCommand(
                userId,
                musicListId,
                targetMusicInfoId,
                prevMusicInfoId,
                nextMusicInfoId
            ),
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(result);
        Assert.NotEqual(200, result.Code);
    }

    [Fact]
    public async Task ArgsError()
    {
        await PrepareAsync();
        var targetMusicInfoId = musicInfoIds[1];
        var prevMusicInfoId = musicInfoIds[1];
        var nextMusicInfoId = musicInfoIds[2];
        var result = await Mediator.Send(
            new ChangeMusicInfoOrderInMusicListCommand(
                userId,
                musicListId,
                targetMusicInfoId,
                prevMusicInfoId,
                nextMusicInfoId
            ),
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(result);
        Assert.NotEqual(200, result.Code);
    }
}
