using MusicManagementDemo.Application.UseCase.Management.CreateStorage;
using MusicManagementDemo.Application.UseCase.Management.ReadStorage;

namespace FunctionalTesting.Management;

public class ReadStorageTest : BaseTestingClass
{
    [Fact]
    public async Task Normal()
    {
        var createStorageResult = await mediator.Send(
            new CreateStorageCommand("Test", "X:\\storage1"),
            TestContext.Current.CancellationToken
        );
        var storageId = (int)createStorageResult.Data!.GetProperty("Id")!;
        var result = await mediator.Send(
            new ReadStorageQuery(storageId),
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
    }

    [Fact]
    public async Task NotExist()
    {
        var result = await mediator.Send(
            new ReadStorageQuery(114514),
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(result);
        Assert.NotEqual(200, result.Code);
    }
}
