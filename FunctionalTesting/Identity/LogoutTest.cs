using MusicManagementDemo.Application.UseCase.Identity.Login;
using MusicManagementDemo.Application.UseCase.Identity.Logout;
using MusicManagementDemo.Application.UseCase.Identity.Register;

namespace FunctionalTesting.Identity;

public class LogoutTest : BaseTestingClass
{
    [Fact]
    public async Task Normal()
    {
        var tempResult = await mediator.Send(
            new RegisterCommand(Email: "aaa@aaa.com", UserName: "aaa", Password: "Abc@123"),
            TestContext.Current.CancellationToken
        );
        await mediator.Send(
            new LoginCommand(Email: "aaa@aaa.com", Password: "Abc@123"),
            TestContext.Current.CancellationToken
        );
        var result = await mediator.Send(
            new LogoutCommand(UserId: Guid.Parse(tempResult.Data!.ToString()!)),
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
    }
}
