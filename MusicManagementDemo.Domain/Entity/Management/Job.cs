using System.Text.Json.Nodes;

namespace MusicManagementDemo.Domain.Entity.Management;

/// <summary>
/// 任务实体
/// </summary>
public class Job
{
    /// <summary>
    /// 唯一标识
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 任务类型
    /// </summary>
    public JobType Type { get; set; }

    /// <summary>
    /// 任务参数
    /// </summary>
    public JsonNode JobArgs { get; set; } = "{}";

    /// <summary>
    /// 状态
    /// </summary>
    public JobStatus Status { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// 错误信息
    /// </summary>
    public string ErrorMesage { get; set; } = string.Empty;

    /// <summary>
    /// 是否成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 完成时间
    /// </summary>
    public DateTime? CompletedAt { get; set; }
}

public enum JobType
{
    /// <summary>
    /// 未定义
    /// </summary>
    Undefined,
    /// <summary>
    /// 增量扫描音乐文件
    /// </summary>
    ScanIncremental,
}

public enum JobStatus
{
    /// <summary>
    /// 未定义
    /// </summary>
    Undefined,
    /// <summary>
    /// 等待开始
    /// </summary>
    WaitingStart,
    /// <summary>
    /// 运行中
    /// </summary>
    Running,
    /// <summary>
    /// 已完成
    /// </summary>
    Completed,
}
