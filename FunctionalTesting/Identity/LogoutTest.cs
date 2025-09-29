using MusicManagementDemo.Application.UseCase.Identity.Login;
using MusicManagementDemo.Application.UseCase.Identity.Logout;
using MusicManagementDemo.Application.UseCase.Identity.Register;

namespace FunctionalTesting.Identity;

public class LogoutTest : BaseTestingClass
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
        await mediator.Send(
            new LoginCommand(Email: "aaa@aaa.com", Password: "Abc@123"),
            TestContext.Current.CancellationToken
        );
        var result = await mediator.Send(
            new LogoutCommand(userId),
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
    }
}
