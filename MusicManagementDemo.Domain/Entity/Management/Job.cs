using System.Text.Json.Nodes;

namespace MusicManagementDemo.Domain.Entity.Management;

public class Job
{
    public long Id { get; set; }

    public JobType Type { get; set; }

    /// <summary>
    /// json
    /// </summary>
    public JsonNode JobArgs { get; set; } = "{}";

    public JobStatus Status { get; set; }

    public string Description { get; set; } = string.Empty;

    public string ErrorMesage { get; set; } = string.Empty;

    public bool Success { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? CompletedAt { get; set; }
}

public enum JobType
{
    Undefined,
    ScanIncremental,
}

public enum JobStatus
{
    Undefined,
    WaitingStart,
    Running,
    Completed,
}
