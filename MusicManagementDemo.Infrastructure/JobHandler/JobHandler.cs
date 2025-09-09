using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MusicManagementDemo.Infrastructure.Database;
using MusicManagementDemo.SharedKernel;

namespace MusicManagementDemo.Infrastructure.JobHandler;

internal sealed class JobHandler(IServiceProvider service) : IJobHandler
{
    public async Task Handle(long jobId, JobType jobType)
    {
        var success = false;
        switch (jobType)
        {
            case JobType.Undefined:
                break;
            case JobType.ScanIncremental:
                success = await HandleScanIncremental();
                break;
            default:
                return;
        }

        await using var scope = service.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ManagementAppDbContext>();
        var job = await dbContext.Job.SingleOrDefaultAsync(e => e.Id == jobId);
        if (job is null)
        {
            return;
        }
        job.CompletedAt = DateTime.UtcNow;
        job.Status = JobStatus.Completed;
        job.Success = success;
        dbContext.Job.Update(job);
        await dbContext.SaveChangesAsync();
    }

    private async Task<bool> HandleScanIncremental()
    {
        await Task.Delay(TimeSpan.FromMinutes(1));
        return true;
    }
}
