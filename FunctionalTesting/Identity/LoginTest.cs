using MusicManagementDemo.Application.UseCase.Identity.Login;
using MusicManagementDemo.Application.UseCase.Identity.Register;

namespace FunctionalTesting.Identity;

public class LoginTest : BaseTestingClass
{
    private Guid userId;

    private async Task PrepareAsync()
    {
        var regResult = await Mediator.Send(
            new RegisterCommand(Email: "aaa@aaa.com", UserName: "aaa", Password: "Abc@123"),
            TestContext.Current.CancellationToken
        );
        userId = Guid.Parse(regResult.Data!.GetPropertyValue("Id")!.ToString()!);
    }

    [Fact]
    public async Task Normal()
    {
        await PrepareAsync();
        var result = await Mediator.Send(
            new LoginCommand(Email: "aaa@aaa.com", Password: "Abc@123"),
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
    }

    [Fact]
    public async Task PasswordError()
    {
        await Mediator.Send(
            new RegisterCommand(Email: "aaa@aaa.com", UserName: "aaa", Password: "Abc@123"),
            TestContext.Current.CancellationToken
        );
        var result = await Mediator.Send(
            new LoginCommand(Email: "aaa@aaa.com", Password: "Abc@123!"),
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(result);
        Assert.NotEqual(200, result.Code);
    }
}
