using MusicManagementDemo.Application.UseCase.Management.CreateStorage;
using MusicManagementDemo.Application.UseCase.Management.ReadAllStorage;

namespace FunctionalTesting.Management;

public class ReadAllStorageTest : BaseTestingClass
{
    public record ThreeAscWithReferenceIdArgs(bool IsReferenceIdNull, bool Asc);

    public static TheoryData<ThreeAscWithReferenceIdArgs> ThreeAscWithReferenceIdTestData =>
        [
            new(new(false, false)),
            new(new(false, true)),
            new(new(true, false)),
            new(new(true, true)),
        ];

    private List<int> storageIds = [];

    private async Task PrepareAsync()
    {
        var result = await Mediator.Send(
            new CreateStorageCommand("a", "X:\\a"),
            TestContext.Current.CancellationToken
        );
        storageIds.Add((int)result.Data!.GetPropertyValue("Id")!);
        result = await Mediator.Send(
            new CreateStorageCommand("b", "X:\\b"),
            TestContext.Current.CancellationToken
        );
        storageIds.Add((int)result.Data!.GetPropertyValue("Id")!);
        result = await Mediator.Send(
            new CreateStorageCommand("c", "X:\\c"),
            TestContext.Current.CancellationToken
        );
        storageIds.Add((int)result.Data!.GetPropertyValue("Id")!);
    }

    [Fact]
    public async Task Empty()
    {
        var result = await Mediator.Send(
            new ReadAllStorageQuery(null, 10, false),
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
    }

    [Fact]
    public async Task ZeroPageSize()
    {
        var result = await Mediator.Send(
            new ReadAllStorageQuery(null, 0, false),
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(result);
        Assert.NotEqual(200, result.Code);
    }

    [Fact]
    public async Task MaxPageSize()
    {
        var result = await Mediator.Send(
            new ReadAllStorageQuery(null, 30, false),
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(result);
        Assert.NotEqual(200, result.Code);
    }

    [Theory]
    [MemberData(nameof(ThreeAscWithReferenceIdTestData))]
    public async Task ThreeAscWithReferenceId(ThreeAscWithReferenceIdArgs args)
    {
        await PrepareAsync();
        var result = await Mediator.Send(
            new ReadAllStorageQuery(args.IsReferenceIdNull ? null : storageIds[1], 10, args.Asc),
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
    }
}
