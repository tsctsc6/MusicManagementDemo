using System.Text.Json.Nodes;
using MusicManagementDemo.Application.UseCase.Identity.Register;
using MusicManagementDemo.Application.UseCase.Management.CreateJob;
using MusicManagementDemo.Application.UseCase.Management.CreateStorage;
using MusicManagementDemo.Application.UseCase.Music.AddMusicInfoToMusicList;
using MusicManagementDemo.Application.UseCase.Music.CreateMusicList;
using MusicManagementDemo.Application.UseCase.Music.GetAllMusicInfoFromMusicList;
using MusicManagementDemo.Application.UseCase.Music.ReadAllMusicInfo;
using MusicManagementDemo.Domain.Entity.Management;

namespace FunctionalTesting.Music;

public class GetAllMusicInfoFromMusicListTest : BaseTestingClass
{
    private Guid userId;
    private Guid musicListId;
    private Guid[] musicInfoIds = [];

    private async Task PrepareAsync(int addToMusicListCount)
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
        for (int i = 0; i < addToMusicListCount; i++)
        {
            await mediator.Send(
                new AddMusicInfoToMusicListCommand(userId, musicListId, musicInfoIds[i]),
                TestContext.Current.CancellationToken
            );
        }
    }

    public record NormalArgs(int AddToMusicListCount, bool IsReferenceIdNull, bool Asc);

    private static readonly int[] AddToMusicListCountList = [1, 3];
    private static readonly bool[] BoolValues = [false, true];

    public static IEnumerable<TheoryDataRow<NormalArgs>> NormalTestData
    {
        get
        {
            foreach (var i in AddToMusicListCountList)
            {
                foreach (var b0 in BoolValues)
                {
                    foreach (var b1 in BoolValues)
                    {
                        yield return new(new(i, b0, b1));
                    }
                }
            }
        }
    }

    [Theory]
    [MemberData(nameof(NormalTestData))]
    public async Task Normal(NormalArgs args)
    {
        await PrepareAsync(args.AddToMusicListCount);
        var result = await mediator.Send(
            new GetAllMusicInfoFromMusicListQuery(
                userId,
                musicListId,
                10,
                args.IsReferenceIdNull ? null : musicInfoIds[1],
                args.Asc
            ),
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
    }

    [Fact]
    public async Task Empty()
    {
        await PrepareAsync(0);
        var result = await mediator.Send(
            new GetAllMusicInfoFromMusicListQuery(userId, musicListId, 10, null, false),
            TestContext.Current.CancellationToken
        );
        ;
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
    }

    [Fact]
    public async Task MinPageSize()
    {
        await PrepareAsync(3);
        var result = await mediator.Send(
            new GetAllMusicInfoFromMusicListQuery(userId, musicListId, 0, null, false),
            TestContext.Current.CancellationToken
        );
        ;
        Assert.NotNull(result);
        Assert.NotEqual(200, result.Code);
    }

    [Fact]
    public async Task MaxPageSize()
    {
        await PrepareAsync(3);
        var result = await mediator.Send(
            new GetAllMusicInfoFromMusicListQuery(userId, musicListId, 30, null, false),
            TestContext.Current.CancellationToken
        );
        ;
        Assert.NotNull(result);
        Assert.NotEqual(200, result.Code);
    }
}
