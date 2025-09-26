using MusicManagementDemo.Application.UseCase.Identity.Logout;
using MusicManagementDemo.Application.UseCase.Management.CreateStorage;

namespace FunctionalTesting.Management;

public class CreateStorageTest : BaseTestingClass
{
    [Fact]
    public async Task Normal()
    {
        var result = await mediator.Send(
            new CreateStorageCommand("Test", "X:\\storage1"),
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
    }

    [Fact]
    public async Task Repeatedly()
    {
        await mediator.Send(
            new CreateStorageCommand("Test", "X:\\storage1"),
            TestContext.Current.CancellationToken
        );
        var result = await mediator.Send(
            new CreateStorageCommand("Test", "X:\\storage1"),
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(result);
        Assert.NotEqual(200, result.Code);
    }
}
