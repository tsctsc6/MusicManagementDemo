namespace MusicManagementDemo.SharedKernel;

public interface IJobHandler
{
    public Task Handle(long jobId, JobType jobType);
}
