using RustSharp;

namespace MusicManagementDemo.SharedKernel;

public interface IJobManager
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="jobId"></param>
    /// <param name="jobType"></param>
    /// <returns>(jobId, errorMessage)</returns>
    public Result<long, string> CreateJob(long jobId, JobType jobType);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="jobId"></param>
    /// <returns>(jobId, errorMessage)</returns>
    public Result<long, string> CancelJob(long jobId);
}
