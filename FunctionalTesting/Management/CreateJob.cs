using System.Text.Json.Nodes;
using MusicManagementDemo.Application.UseCase.Management.CreateJob;
using MusicManagementDemo.Application.UseCase.Management.CreateStorage;
using MusicManagementDemo.Application.UseCase.Management.ReadJob;
using MusicManagementDemo.Domain.Entity.Management;

namespace FunctionalTesting.Management;

public class CreateJob : BaseTestingClass
{
    [Fact]
    public async Task Normal()
    {
        var createStorageResult = await mediator.Send(
            new CreateStorageCommand("Test", "X:\\storage1"),
            TestContext.Current.CancellationToken
        );
        var storageId = (int)createStorageResult.Data!;
        var result = await mediator.Send(
            new CreateJobCommand(
                JobType.ScanIncremental,
                "ddd",
                JsonNode.Parse($"{{\"storageId\": {storageId}}}")!
            ),
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
    }

    [Fact]
    public async Task StorageNotExist()
    {
        var createJobResult = await mediator.Send(
            new CreateJobCommand(
                JobType.ScanIncremental,
                "ddd",
                JsonNode.Parse($"{{\"storageId\": 114514}}")!
            ),
            TestContext.Current.CancellationToken
        );
        var jobId = (long)createJobResult.Data?.GetProperty("JobId")!;
        await Task.Delay(TimeSpan.FromSeconds(6), TestContext.Current.CancellationToken);
        var result = await mediator.Send(
            new ReadJobQuery(jobId),
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(result);
        var completedAt = result.Data?.GetProperty("CompletedAt");
        var success = result.Data?.GetProperty("Success");
        Assert.NotNull(completedAt);
        Assert.Equal(false, success);
    }

    [Fact]
    public async Task StoragePathNotFound()
    {
        var createStorageResult = await mediator.Send(
            new CreateStorageCommand("Test", "X:\\storage13"),
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
        await Task.Delay(TimeSpan.FromSeconds(6), TestContext.Current.CancellationToken);
        var result = await mediator.Send(
            new ReadJobQuery(jobId),
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(result);
        var completedAt = result.Data?.GetProperty("CompletedAt");
        var success = result.Data?.GetProperty("Success");
        Assert.NotNull(completedAt);
        Assert.Equal(false, success);
    }
}
