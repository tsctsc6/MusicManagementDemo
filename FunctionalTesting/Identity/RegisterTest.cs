using MusicManagementDemo.Application.UseCase.Identity.Register;

namespace FunctionalTesting.Identity;

public class RegisterTest : BaseTestingClass
{
    [Fact]
    public async Task Success()
    {
        var result = await mediator.Send(
            new RegisterCommand(Email: "aaa@aaa.com", UserName: "aaa", Password: "Abc@123"),
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
    }

    [Fact]
    public async Task Repeatedly()
    {
        await mediator.Send(
            new RegisterCommand(Email: "aaa@aaa.com", UserName: "aaa", Password: "Abc@123"),
            TestContext.Current.CancellationToken
        );
        var result = await mediator.Send(
            new RegisterCommand(Email: "aaa@aaa.com", UserName: "aaa", Password: "Abc@123"),
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(result);
        Assert.NotEqual(200, result.Code);
    }

    [Fact]
    public async Task SimplePassword()
    {
        var result = await mediator.Send(
            new RegisterCommand(Email: "aaa@aaa.com", UserName: "aaa", Password: "12345678"),
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(result);
        Assert.NotEqual(200, result.Code);
    }
}
