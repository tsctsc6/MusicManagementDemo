using MusicManagementDemo.SharedKernel;

namespace MusicManagementDemo.Domain.Entity.Management;

public class Job : SharedKernel.Entity
{
    public long Id { get; set; }

    /// <summary>
    /// json
    /// </summary>
    public JobType Type { get; set; }

    public string JobArgs { get; set; } = "{}";

    public JobStatus Status { get; set; }

    public string Description { get; set; } = string.Empty;

    public bool Success { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? CompletedAt { get; set; }
}
