using FunctionalTesting.Provisions;
using MusicManagementDemo.Application.UseCase.Management.CreateStorage;
using MusicManagementDemo.Application.UseCase.Management.ReadStorage;

namespace FunctionalTesting.Management;

public class ReadStorageTest : BaseTestingClass
{
    private int storageId;

    private async Task PrepareAsync()
    {
        storageId = await ManagementProvision.CreateStorageAsync(
            Mediator,
            new CreateStorageCommand("Test", "X:\\storage1"),
            TestContext.Current.CancellationToken
        );
    }

    [Fact]
    public async Task Normal()
    {
        await PrepareAsync();
        var result = await Mediator.Send(
            new ReadStorageQuery(storageId),
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
    }

    [Fact]
    public async Task NotExist()
    {
        var result = await Mediator.Send(
            new ReadStorageQuery(0721),
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(result);
        Assert.NotEqual(200, result.Code);
    }
}
