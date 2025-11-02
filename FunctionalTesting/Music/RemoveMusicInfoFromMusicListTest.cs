using System.Text.Json.Nodes;
using FunctionalTesting.Provisions;
using MusicManagementDemo.Application.UseCase.Identity.Register;
using MusicManagementDemo.Application.UseCase.Management.CreateJob;
using MusicManagementDemo.Application.UseCase.Management.CreateStorage;
using MusicManagementDemo.Application.UseCase.Music.AddMusicInfoToMusicList;
using MusicManagementDemo.Application.UseCase.Music.CreateMusicList;
using MusicManagementDemo.Application.UseCase.Music.ReadAllMusicInfo;
using MusicManagementDemo.Application.UseCase.Music.RemoveMusicInfoFromMusicList;
using MusicManagementDemo.Domain.Entity.Management;

namespace FunctionalTesting.Music;

public class RemoveMusicInfoFromMusicListTest : BaseTestingClass
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
        for (var i = 0; i < 6; i++)
        {
            await MusicProvision.AddMusicInfoToMusicListAsync(
                Mediator,
                new AddMusicInfoToMusicListCommand(userId, musicListId, musicInfoIds[i]),
                TestContext.Current.CancellationToken
            );
        }
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(5)]
    public async Task Normal(int removeIndex)
    {
        await PrepareAsync();
        var result = await Mediator.Send(
            new RemoveMusicInfoFromMusicListCommand(userId, musicListId, musicInfoIds[removeIndex]),
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
    }

    [Fact]
    public async Task MusicInfoNotExist()
    {
        await PrepareAsync();
        var result = await Mediator.Send(
            new RemoveMusicInfoFromMusicListCommand(userId, musicListId, Guid.Empty),
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
            new RemoveMusicInfoFromMusicListCommand(userId, Guid.Empty, musicInfoIds[1]),
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(result);
        Assert.NotEqual(200, result.Code);
    }
}
