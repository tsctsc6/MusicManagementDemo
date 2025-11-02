using System.Text.Json.Nodes;
using FunctionalTesting.Provisions;
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
    private Guid[] musicInfoIds = [];

    private async Task PrepareAsync()
    {
        userId = await IdentityProvision.RegisterAsync(
            Mediator,
            new RegisterCommand(Email: "aaa@aaa.com", UserName: "aaa", Password: "Abc@123"),
            TestContext.Current.CancellationToken
        );
        musicListId = await MusicProvision.CreateMusicListAsync(
            Mediator,
            new CreateMusicListCommand(userId, "New MusicList"),
            TestContext.Current.CancellationToken
        );
        var storageId = await ManagementProvision.CreateStorageAsync(
            Mediator,
            new CreateStorageCommand("Test", "X:\\storage1"),
            TestContext.Current.CancellationToken
        );
        var jobId = await ManagementProvision.CreateJobAsync(
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

    [Fact]
    public async Task Normal()
    {
        await PrepareAsync();
        var result = await Mediator.Send(
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
        await Mediator.Send(
            new AddMusicInfoToMusicListCommand(userId, musicListId, musicInfoIds[0]),
            TestContext.Current.CancellationToken
        );
        var result = await Mediator.Send(
            new AddMusicInfoToMusicListCommand(userId, musicListId, musicInfoIds[0]),
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(result);
        Assert.NotEqual(200, result.Code);
    }

    [Fact]
    public async Task UserNotExist()
    {
        await PrepareAsync();
        var result = await Mediator.Send(
            new AddMusicInfoToMusicListCommand(Guid.Empty, musicListId, musicInfoIds[0]),
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(result);
        Assert.NotEqual(200, result.Code);
    }

    [Fact]
    public async Task MusicListNotExist()
    {
        await PrepareAsync();
        var result = await Mediator.Send(
            new AddMusicInfoToMusicListCommand(userId, Guid.Empty, musicInfoIds[0]),
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(result);
        Assert.NotEqual(200, result.Code);
    }

    [Fact]
    public async Task MusicInfoNotExist()
    {
        await PrepareAsync();
        var result = await Mediator.Send(
            new AddMusicInfoToMusicListCommand(userId, musicListId, Guid.Empty),
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(result);
        Assert.NotEqual(200, result.Code);
    }
}
