using System.Text.Json.Nodes;
using MusicManagementDemo.Application.UseCase.Management.CreateJob;
using MusicManagementDemo.Application.UseCase.Management.CreateStorage;
using MusicManagementDemo.Application.UseCase.Music.ReadAllMusicInfo;
using MusicManagementDemo.Domain.Entity.Management;
using static FunctionalTesting.Management.ReadAllStorageTest;

namespace FunctionalTesting.Music;

public class ReadAllMusicInfoTest : BaseTestingClass
{
    private Guid[] musicInfoIds = [];

    private async Task PrepareAsync()
    {
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

    public record NormalArgs(bool IsReferenceIdNull, bool Asc);

    public static IEnumerable<TheoryDataRow<NormalArgs>> NormalTestData =>
        [
            new(new(false, false)),
            new(new(false, true)),
            new(new(true, false)),
            new(new(true, true)),
        ];

    [Theory]
    [MemberData(nameof(NormalTestData))]
    public async Task Normal(NormalArgs args)
    {
        await PrepareAsync();
        var result = await mediator.Send(
            new ReadAllMusicInfoQuery(
                args.IsReferenceIdNull ? null : musicInfoIds[1],
                10,
                args.Asc,
                string.Empty
            ),
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
    }

    [Fact]
    public async Task MinPageSize()
    {
        await PrepareAsync();
        var result = await mediator.Send(
            new ReadAllMusicInfoQuery(null, 0, false, string.Empty),
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(result);
        Assert.NotEqual(200, result.Code);
    }

    [Fact]
    public async Task MaxPageSize()
    {
        await PrepareAsync();
        var result = await mediator.Send(
            new ReadAllMusicInfoQuery(null, 30, false, string.Empty),
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(result);
        Assert.NotEqual(200, result.Code);
    }

    [Fact]
    public async Task Empty()
    {
        await PrepareAsync();
        var result = await mediator.Send(
            new ReadAllMusicInfoQuery(null, 10, false, string.Empty),
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
    }

    [Fact]
    public async Task Search()
    {
        var result = await mediator.Send(
            new ReadAllMusicInfoQuery(null, 10, false, "right asx"),
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
    }
}
