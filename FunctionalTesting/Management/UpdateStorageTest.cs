using System.Text;
using MusicManagementDemo.Application.UseCase.Management.CreateStorage;
using MusicManagementDemo.Application.UseCase.Management.UpdateStorage;

namespace FunctionalTesting.Management;

public class UpdateStorageTest : BaseTestingClass
{
    private int storageId;

    private async Task PrepareAsync()
    {
        var createStorageResult = await Mediator.Send(
            new CreateStorageCommand("Test", "X:\\storage1"),
            TestContext.Current.CancellationToken
        );
        storageId = (int)createStorageResult.Data!.GetPropertyValue("Id")!;
    }

    [Fact]
    public async Task Normal()
    {
        await PrepareAsync();
        var result = await Mediator.Send(
            new UpdateStorageCommand(storageId, "Test2", "X:\\storage1"),
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
    }

    [Fact]
    public async Task NotExist()
    {
        var result = await Mediator.Send(
            new UpdateStorageCommand(114514, "Test2", "X:\\storage1"),
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(result);
        Assert.NotEqual(200, result.Code);
    }

    [Fact]
    public async Task InvalidName()
    {
        var sb = new StringBuilder();
        for (int i = 0; i < 10; i++)
        {
            sb.Append("abdefghi123");
        }
        await PrepareAsync();
        var result = await Mediator.Send(
            new UpdateStorageCommand(storageId, sb.ToString(), "X:\\storage1"),
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(result);
        Assert.NotEqual(200, result.Code);
    }

    [Fact]
    public async Task InvalidPath()
    {
        var sb = new StringBuilder("X:\\");
        for (int i = 0; i < 22; i++)
        {
            sb.Append("abdefghi123\\");
        }
        await PrepareAsync();
        var result = await Mediator.Send(
            new UpdateStorageCommand(storageId, "Test", sb.ToString()),
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(result);
        Assert.NotEqual(200, result.Code);
    }
}
