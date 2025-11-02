using MusicManagementDemo.Application.UseCase.Management.CreateStorage;
using MusicManagementDemo.Application.UseCase.Management.DeleteStorage;

namespace FunctionalTesting.Management;

public class DeleteStorageTest : BaseTestingClass
{
    [Fact]
    public async Task Normal()
    {
        var createStorageResult = await Mediator.Send(
            new CreateStorageCommand("Test", "X:\\storage1"),
            TestContext.Current.CancellationToken
        );
        var storageId = (int)createStorageResult.Data!.GetProperty("Id")!;
        var result = await Mediator.Send(
            new DeleteStorageCommand(storageId),
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
    }

    [Fact]
    public async Task StorageNotExist()
    {
        var result = await Mediator.Send(
            new DeleteStorageCommand(0x0d00),
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(result);
        Assert.NotEqual(200, result.Code);
    }
}
