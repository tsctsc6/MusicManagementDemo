namespace MusicManagementDemo.SharedKernel;

public interface IJobManager
{
    public void CreateJob(long jobId, JobType jobType);

    public void CancelJob(long jobId);
}
