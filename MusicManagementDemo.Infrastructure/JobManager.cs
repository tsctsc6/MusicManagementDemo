using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using MusicManagementDemo.Infrastructure.Database;
using MusicManagementDemo.SharedKernel;

namespace MusicManagementDemo.Infrastructure;

internal sealed class JobManager(IServiceProvider service) : IJobManager
{
    private readonly ConcurrentDictionary<long, Task<long>> tasks = [];
    private readonly ConcurrentDictionary<long, CancellationTokenSource> cancellationTokenSources =
    [];

    public void CreateJob(long jobId, JobType jobType)
    {
        var cts = new CancellationTokenSource();
        switch (jobType)
        {
            case JobType.ScanIncremental:
                // jobId 来自数据库，应该是不会重复的
                cancellationTokenSources.TryAdd(jobId, cts);
                tasks.TryAdd(
                    jobId,
                    (
                        HandleScanIncremental(jobId, cts.Token).ContinueWith(TaskContinueWith)
                        as Task<long>
                    )!
                );
                break;
            case JobType.Undefined:
            default:
                return;
        }
        using var scope = service.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<ManagementAppDbContext>();
        var jobToUpdate = dbContext.Job.SingleOrDefault(e => e.Id == jobId);
        if (jobToUpdate is null) return;
        jobToUpdate.Status = JobStatus.Running;
        dbContext.Job.Update(jobToUpdate);
        dbContext.SaveChanges();
    }

    public void CancelJob(long jobId)
    {
        cancellationTokenSources[jobId].Cancel();
    }

    private void TaskContinueWith(Task<long> task)
    {
        var jobId= task.Result;
        using var scope = service.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<ManagementAppDbContext>();
        var jobToUpdate = dbContext.Job.SingleOrDefault(e => e.Id == jobId);
        if (jobToUpdate is null) return;
        if (task.IsCompleted)
        {
            jobToUpdate.Status = JobStatus.Completed;
            jobToUpdate.Success = task.IsCompletedSuccessfully;
            jobToUpdate.CompletedAt = DateTime.UtcNow;
        }
        dbContext.Job.Update(jobToUpdate);
        dbContext.SaveChanges();
        tasks.TryRemove(jobId, out _);
        task.Dispose();
        cancellationTokenSources.TryRemove(jobId, out var cts);
        cts?.Dispose();
    }

    private async Task<long> HandleScanIncremental(long jobId, CancellationToken token)
    {
        await Task.Delay(TimeSpan.FromSeconds(30), token);
        return jobId;
    }
}
