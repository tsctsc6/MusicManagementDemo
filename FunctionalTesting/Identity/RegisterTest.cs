using MusicManagementDemo.Application.UseCase.Identity.Register;

namespace FunctionalTesting.Identity;

public class RegisterTest() : BaseTestingClass
{
    [Fact]
    public async Task Test()
    {
        var result = await mediator.Send(
            new RegisterCommand(Email: "aaa@aaa.com", UserName: "aaa", Password: "Abc@123"),
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
    }
}
