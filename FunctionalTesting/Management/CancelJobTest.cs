using System.Text.Json.Nodes;
using MusicManagementDemo.Application.UseCase.Management.CancelJob;
using MusicManagementDemo.Application.UseCase.Management.CreateJob;
using MusicManagementDemo.Application.UseCase.Management.CreateStorage;
using MusicManagementDemo.Domain.Entity.Management;

namespace FunctionalTesting.Management;

public class CancelJobTest : BaseTestingClass
{
    [Fact]
    public async Task Normal()
    {
        var createStorageResult = await Mediator.Send(
            new CreateStorageCommand("Test", "X:\\storage1"),
            TestContext.Current.CancellationToken
        );
        var storageId = (int)createStorageResult.Data!.GetPropertyValue("Id")!;
        var createJobResult = await Mediator.Send(
            new CreateJobCommand(
                JobType.ScanIncremental,
                "ddd",
                JsonNode.Parse($"{{\"storageId\": {storageId}}}")!
            ),
            TestContext.Current.CancellationToken
        );
        var jobId = (long)createJobResult.Data?.GetPropertyValue("JobId")!;
        await Task.Delay(TimeSpan.FromSeconds(1), TestContext.Current.CancellationToken);
        var result = await Mediator.Send(
            new CancelJobCommand(jobId),
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
    }

    [Fact]
    public async Task NotExist()
    {
        var result = await Mediator.Send(
            new CancelJobCommand(114514),
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(result);
        Assert.NotEqual(200, result.Code);
    }
}
