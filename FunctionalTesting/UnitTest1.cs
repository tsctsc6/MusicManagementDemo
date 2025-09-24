using MusicManagementDemo.Abstractions.IDbContext;

namespace FunctionalTesting;

public class UnitTest1
{
    private readonly IMusicAppDbContext _dbContext;

    public UnitTest1(IMusicAppDbContext dbContext)
    {
        _dbContext = dbContext;
        _dbContext.Database.EnsureCreated();
    }

    [Fact]
    public void Test1() { }
}
