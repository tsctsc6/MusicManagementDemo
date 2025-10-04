using MusicManagementDemo.Application.UseCase.Identity.Register;
using MusicManagementDemo.Application.UseCase.Music.CreateMusicList;
using MusicManagementDemo.Application.UseCase.Music.UpdateMusicList;
using System.Text;
using Xunit.Sdk;

namespace FunctionalTesting.Music;

public class UpdateMusicListTest : BaseTestingClass
{
    private Guid userId;
    private Guid musicListId;

    private async Task PrepareAsync()
    {
        var regResult = await mediator.Send(
            new RegisterCommand(Email: "aaa@aaa.com", UserName: "aaa", Password: "Abc@123"),
            TestContext.Current.CancellationToken
        );
        userId = Guid.Parse(regResult.Data!.GetProperty("Id")!.ToString()!);
        var createMusicListResult = await mediator.Send(
            new CreateMusicListCommand(userId, "New MusicList"),
            TestContext.Current.CancellationToken
        );
        musicListId = Guid.Parse(createMusicListResult.Data!.GetProperty("Id")!.ToString()!);
    }

    [Fact]
    public async Task Normal()
    {
        await PrepareAsync();
        var result = await mediator.Send(
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
        var result = await mediator.Send(
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
        for (int i = 0; i < 8; i++)
        {
            sb.Append("New MusicList2");
        }
        var result = await mediator.Send(
            new UpdateMusicListCommand(userId, musicListId, sb.ToString()),
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(result);
        Assert.NotEqual(200, result.Code);
    }
}
