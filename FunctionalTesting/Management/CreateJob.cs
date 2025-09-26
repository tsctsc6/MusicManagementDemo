using System.Text.Json.Nodes;
using MusicManagementDemo.Application.UseCase.Management.CreateJob;
using MusicManagementDemo.Application.UseCase.Management.CreateStorage;
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
}
