namespace MusicManagementDemo.Domain.Entity.Music;

/// <summary>
/// 音乐信息和歌单的对应表
/// </summary>
public class MusicInfoMusicListMap
{
    /// <summary>
    /// 音乐信息 id
    /// </summary>
    public Guid MusicInfoId { get; set; }

    /// <summary>
    /// 歌单 id
    /// </summary>
    public Guid MusicListId { get; set; }

    /// <summary>
    /// 用于排序的值，使用 LexoRank 方法，base62
    /// </summary>
    public string SortingOrder { get; set; } = string.Empty;

    /// <summary>
    /// 加入歌单时间
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
