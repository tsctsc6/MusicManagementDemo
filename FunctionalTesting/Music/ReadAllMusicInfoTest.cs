using System.Text;
using System.Text.Json.Nodes;
using FunctionalTesting.Provisions;
using MusicManagementDemo.Application.UseCase.Management.CreateJob;
using MusicManagementDemo.Application.UseCase.Management.CreateStorage;
using MusicManagementDemo.Application.UseCase.Music.ReadAllMusicInfo;
using MusicManagementDemo.Domain.Entity.Management;

namespace FunctionalTesting.Music;

public class ReadAllMusicInfoTest : BaseTestingClass
{
    private Guid[] musicInfoIds = [];

    private async Task PrepareAsync()
    {
        var storageId = await ManagementProvision.CreateStorageAsync(
            Mediator,
            new CreateStorageCommand("Test", "X:\\storage1"),
            TestContext.Current.CancellationToken
        );
        _ = await ManagementProvision.CreateJobAsync(
            Mediator,
            new CreateJobCommand(
                JobType.ScanIncremental,
                "ddd",
                JsonNode.Parse($"{{\"storageId\": {storageId}}}")!
            ),
            TestContext.Current.CancellationToken
        );
        await Task.Delay(TimeSpan.FromSeconds(6), TestContext.Current.CancellationToken);
        musicInfoIds = await MusicProvision.ReadAllMusicInfoAsync(
            Mediator,
            new ReadAllMusicInfoQuery(null, 10, false, string.Empty),
            TestContext.Current.CancellationToken
        );
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public async Task Normal(bool isReferenceIdNull, bool asc)
    {
        await PrepareAsync();
        var result = await Mediator.Send(
            new ReadAllMusicInfoQuery(
                isReferenceIdNull ? null : musicInfoIds[1],
                10,
                asc,
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
        var result = await Mediator.Send(
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
        var result = await Mediator.Send(
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
        var result = await Mediator.Send(
            new ReadAllMusicInfoQuery(null, 10, false, string.Empty),
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
    }

    [Fact]
    public async Task Search()
    {
        var result = await Mediator.Send(
            new ReadAllMusicInfoQuery(null, 10, false, "right asx"),
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
    }

    [Fact]
    public async Task InvalidSearchTerm()
    {
        var sb = new StringBuilder();
        for (var i = 0; i < 20; i++)
        {
            sb.Append("abcdefgh123");
        }
        var result = await Mediator.Send(
            new ReadAllMusicInfoQuery(null, 10, false, sb.ToString()),
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(result);
        Assert.NotEqual(200, result.Code);
    }
}
