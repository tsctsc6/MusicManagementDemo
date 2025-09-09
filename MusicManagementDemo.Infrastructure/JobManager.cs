using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using MusicManagementDemo.Infrastructure.Database;
using MusicManagementDemo.SharedKernel;

namespace MusicManagementDemo.Infrastructure;

internal sealed class JobManager(IServiceProvider service) : IJobManager
{
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
                HandleScanIncremental(jobId, cts.Token)
                    .ContinueWith(task =>
                    {
                        using var scope = service.CreateScope();
                        using var dbContext =
                            scope.ServiceProvider.GetRequiredService<ManagementAppDbContext>();
                        var jobToUpdate = dbContext.Job.SingleOrDefault(e => e.Id == jobId);
                        if (jobToUpdate is null)
                            return;
                        if (task.IsCompleted)
                        {
                            jobToUpdate.Success = task.IsCompletedSuccessfully;
                        }
                        else if (task.IsFaulted || task.IsCanceled)
                        {
                            jobToUpdate.Success = false;
                        }

                        jobToUpdate.Status = JobStatus.Completed;
                        jobToUpdate.CompletedAt = DateTime.UtcNow;
                        dbContext.Job.Update(jobToUpdate);
                        dbContext.SaveChanges();
                        task.Dispose();
                        cts.Dispose();
                    });
                break;
            case JobType.Undefined:
            default:
                return;
        }
        using var scope = service.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<ManagementAppDbContext>();
        var jobToUpdate = dbContext.Job.SingleOrDefault(e => e.Id == jobId);
        if (jobToUpdate is null)
            return;
        jobToUpdate.Status = JobStatus.Running;
        dbContext.Job.Update(jobToUpdate);
        dbContext.SaveChanges();
    }

    public void CancelJob(long jobId)
    {
        cancellationTokenSources.TryGetValue(jobId, out var cts);
        cts?.Cancel();
    }

    private async Task<long> HandleScanIncremental(long jobId, CancellationToken token)
    {
        await Task.Delay(TimeSpan.FromSeconds(30), token);
        return jobId;
    }
}
