using System.Text.Json.Nodes;
using FunctionalTesting.Provisions;
using MusicManagementDemo.Application.UseCase.Management.CreateJob;
using MusicManagementDemo.Application.UseCase.Management.CreateStorage;
using MusicManagementDemo.Application.UseCase.Management.ReadJob;
using MusicManagementDemo.Domain.Entity.Management;

namespace FunctionalTesting.Management;

public class ReadJobTest : BaseTestingClass
{
    private int storageId;
    private long jobId;

    private async Task PrepareAsync()
    {
        storageId = await ManagementProvision.CreateStorageAsync(
            Mediator,
            new CreateStorageCommand("Test", "X:\\storage1"),
            TestContext.Current.CancellationToken
        );
        jobId = await ManagementProvision.CreateJobAsync(
            Mediator,
            new CreateJobCommand(
                JobType.ScanIncremental,
                "ddd",
                JsonNode.Parse($"{{\"storageId\": {storageId}}}")!
            ),
            TestContext.Current.CancellationToken
        );
    }

    [Fact]
    public async Task Normal()
    {
        await PrepareAsync();
        await Task.Delay(TimeSpan.FromSeconds(1), TestContext.Current.CancellationToken);
        var result = await Mediator.Send(
            new ReadJobQuery(jobId),
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
    }

    [Fact]
    public async Task NotExist()
    {
        var result = await Mediator.Send(
            new ReadJobQuery(114514),
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(result);
        Assert.NotEqual(200, result.Code);
    }
}
