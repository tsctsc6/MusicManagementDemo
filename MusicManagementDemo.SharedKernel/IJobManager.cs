namespace MusicManagementDemo.SharedKernel;

public interface IJobManager
{
    public void AddJob(long jobId, JobType jobType);
}
