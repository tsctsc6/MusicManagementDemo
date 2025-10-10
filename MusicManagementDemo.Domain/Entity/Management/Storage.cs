namespace MusicManagementDemo.Domain.Entity.Management;

/// <summary>
/// 存储
/// </summary>
public class Storage
{
    /// <summary>
    /// 唯一标识
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 路径（目前仅限 Windows 路径）
    /// </summary>
    public string Path { get; set; } = string.Empty;
}
