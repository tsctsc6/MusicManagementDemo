using MusicManagementDemo.Domain.Entity.Management;
using RustSharp;

namespace MusicManagementDemo.Abstractions;

public interface IJobManager
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="jobId"></param>
    /// <param name="jobType"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>(jobId, errorMessage)</returns>
    public Task<Result<long, string>> CreateJobAsync(
        long jobId,
        JobType jobType,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    ///
    /// </summary>
    /// <param name="jobId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>(jobId, errorMessage)</returns>
    public Task<Result<long, string>> CancelJobAsync(long jobId);
}
