namespace MusicManagementDemo.SharedKernel;

public interface IJobManager
{
    public void CreateJob(long jobId, JobType jobType);
}
