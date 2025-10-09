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
    /// 前一首音乐的 id, null 表示这是第一首
    /// </summary>
    public Guid? PrevId { get; set; }

    /// <summary>
    /// 后一首音乐的 id, null 表示这是最后一首
    /// </summary>
    public Guid? NextId { get; set; }

    /// <summary>
    /// 加入歌单时间
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
