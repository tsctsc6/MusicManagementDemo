namespace FunctionalTesting;

public class UnitTest1(IServiceProvider services) : BaseTestingClass(services)
{
    [Fact]
    public void Test1()
    {
        Assert.True(true);
    }
}
