using System.Text;
using FunctionalTesting.Provisions;
using MusicManagementDemo.Application.UseCase.Identity.Register;
using MusicManagementDemo.Application.UseCase.Music.CreateMusicList;
using MusicManagementDemo.Application.UseCase.Music.UpdateMusicList;

namespace FunctionalTesting.Music;

public class UpdateMusicListTest : BaseTestingClass
{
    private Guid userId;
    private Guid musicListId;

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
    }

    [Fact]
    public async Task Normal()
    {
        await PrepareAsync();
        var result = await Mediator.Send(
            new UpdateMusicListCommand(userId, musicListId, "New MusicList2"),
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
    }

    [Fact]
    public async Task NotExist()
    {
        await PrepareAsync();
        var result = await Mediator.Send(
            new UpdateMusicListCommand(userId, Guid.Empty, "New MusicList2"),
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(result);
        Assert.NotEqual(200, result.Code);
    }

    [Fact]
    public async Task InvalidName()
    {
        await PrepareAsync();
        var sb = new StringBuilder();
        for (var i = 0; i < 8; i++)
        {
            sb.Append("New MusicList2");
        }
        var result = await Mediator.Send(
            new UpdateMusicListCommand(userId, musicListId, sb.ToString()),
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(result);
        Assert.NotEqual(200, result.Code);
    }
}
