using MusicManagementDemo.Application.UseCase.Identity.Register;
using MusicManagementDemo.Application.UseCase.Music.CreateMusicList;
using MusicManagementDemo.Application.UseCase.Music.DeleteMusicList;

namespace FunctionalTesting.Music;

public class DeleteMusicListTest : BaseTestingClass
{
    private Guid userId;
    private Guid musicListId;

    private async Task PrepareAsync()
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
    }

    [Fact]
    public async Task Normal()
    {
        await PrepareAsync();
        var result = await mediator.Send(
            new DeleteMusicListCommand(userId, musicListId),
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
    }

    [Fact]
    public async Task MusicListNotExist()
    {
        await PrepareAsync();
        var result = await mediator.Send(
            new DeleteMusicListCommand(userId, Guid.Empty),
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(result);
        Assert.NotEqual(200, result.Code);
    }
}
