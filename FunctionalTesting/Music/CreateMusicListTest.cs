using MusicManagementDemo.Application.UseCase.Identity.Register;
using MusicManagementDemo.Application.UseCase.Music.CreateMusicList;

namespace FunctionalTesting.Music;

public class CreateMusicListTest : BaseTestingClass
{
    private Guid userId;

    private async Task PrepareAsync()
    {
        var regResult = await mediator.Send(
            new RegisterCommand(Email: "aaa@aaa.com", UserName: "aaa", Password: "Abc@123"),
            TestContext.Current.CancellationToken
        );
        userId = Guid.Parse(regResult.Data!.ToString()!);
    }

    [Fact]
    public async Task Normal()
    {
        await PrepareAsync();
        var result = await mediator.Send(
            new CreateMusicListCommand(userId, "New MusicList"),
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
            new CreateMusicListCommand(userId, "New MusicList"),
            TestContext.Current.CancellationToken
        );
        var result = await mediator.Send(
            new CreateMusicListCommand(userId, "New MusicList"),
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
    }
}
