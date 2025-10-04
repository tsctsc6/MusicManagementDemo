using MusicManagementDemo.Application.UseCase.Identity.Register;
using MusicManagementDemo.Application.UseCase.Music.CreateMusicList;
using MusicManagementDemo.Application.UseCase.Music.ReadAllMusicList;

namespace FunctionalTesting.Music;

public class ReadAllMusicListTest : BaseTestingClass
{
    private Guid userId;
    private List<Guid> musicListIds = [];

    private async Task PrepareAsync(int createMusicListCount)
    {
        var regResult = await mediator.Send(
            new RegisterCommand(Email: "aaa@aaa.com", UserName: "aaa", Password: "Abc@123"),
            TestContext.Current.CancellationToken
        );
        userId = Guid.Parse(regResult.Data!.GetProperty("Id")!.ToString()!);
        for (int i = 0; i < createMusicListCount; i++)
        {
            var createMusicListResult = await mediator.Send(
                new CreateMusicListCommand(userId, "New MusicList"),
                TestContext.Current.CancellationToken
            );
            musicListIds.Add(
                Guid.Parse(createMusicListResult.Data!.GetProperty("Id")!.ToString()!)
            );
        }
    }

    public record NormalArgs(bool IsReferenceIdNull, bool Asc);

    public static IEnumerable<TheoryDataRow<NormalArgs>> NormalTestData =>
        [
            new(new(false, false)),
            new(new(false, true)),
            new(new(true, false)),
            new(new(true, true)),
        ];

    [Theory]
    [MemberData(nameof(NormalTestData))]
    public async Task Normal(NormalArgs args)
    {
        await PrepareAsync(3);
        var result = await mediator.Send(
            new ReadAllMusicListQuery(
                userId,
                args.IsReferenceIdNull ? null : musicListIds[1],
                10,
                args.Asc
            ),
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
    }

    [Fact]
    public async Task MinPageSize()
    {
        await PrepareAsync(3);
        var result = await mediator.Send(
            new ReadAllMusicListQuery(userId, null, 0, false),
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(result);
        Assert.NotEqual(200, result.Code);
    }

    [Fact]
    public async Task MaxPageSize()
    {
        await PrepareAsync(3);
        var result = await mediator.Send(
            new ReadAllMusicListQuery(userId, null, 30, false),
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(result);
        Assert.NotEqual(200, result.Code);
    }

    [Fact]
    public async Task Empty()
    {
        await PrepareAsync(0);
        var result = await mediator.Send(
            new ReadAllMusicListQuery(userId, null, 10, false),
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
    }
}
