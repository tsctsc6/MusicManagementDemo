namespace MusicManagementDemo.Domain.Entity.Music;

/// <summary>
/// 歌单
/// </summary>
public class MusicList
{
    /// <summary>
    /// 唯一标识
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 歌单名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 所属用户 id
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
