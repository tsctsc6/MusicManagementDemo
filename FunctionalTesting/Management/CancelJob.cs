using System.Text.Json.Nodes;
using MusicManagementDemo.Application.UseCase.Management.CancelJob;
using MusicManagementDemo.Application.UseCase.Management.CreateJob;
using MusicManagementDemo.Application.UseCase.Management.CreateStorage;
using MusicManagementDemo.Application.UseCase.Management.ReadJob;
using MusicManagementDemo.Domain.Entity.Management;

namespace FunctionalTesting.Management;

public class CancelJob : BaseTestingClass
{
    [Fact]
    public async Task Normal()
    {
        var createStorageResult = await mediator.Send(
            new CreateStorageCommand("Test", "X:\\storage1"),
            TestContext.Current.CancellationToken
        );
        var storageId = (int)createStorageResult.Data!;
        var createJobResult = await mediator.Send(
            new CreateJobCommand(
                JobType.ScanIncremental,
                "ddd",
                JsonNode.Parse($"{{\"storageId\": {storageId}}}")!
            ),
            TestContext.Current.CancellationToken
        );
        var jobId = (long)createJobResult.Data?.GetProperty("JobId")!;
        await Task.Delay(TimeSpan.FromSeconds(1), TestContext.Current.CancellationToken);
        var readJobResult = await mediator.Send(
            new ReadJobQuery(jobId),
            TestContext.Current.CancellationToken
        );
        var result = await mediator.Send(
            new CancelJobCommand(jobId),
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
        // 不知道为什么在单元测试环境不成功，项目实际运行起来就成功。
    }

    [Fact]
    public async Task NotExist()
    {
        var result = await mediator.Send(
            new CancelJobCommand(114514),
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(result);
        Assert.NotEqual(200, result.Code);
    }
}
