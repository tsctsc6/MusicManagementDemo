using NpgsqlTypes;

namespace MusicManagementDemo.Domain.Entity.Music;

/// <summary>
/// 音乐信息
/// </summary>
public sealed class MusicInfo
{
    /// <summary>
    /// 唯一标识
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 音乐名称
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 参与创作的艺术家
    /// </summary>
    public string Artist { get; set; } = string.Empty;

    public string Album { get; set; } = string.Empty;

    /// <summary>
    /// 相对于存储路径的相对路径
    /// </summary>
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// 对应的存储 id
    /// </summary>
    public int StorageId { get; set; }

    /// <summary>
    /// 音乐名称的文本搜索向量，是存储生成列，由 pgsql 根据音乐名称自动计算。
    /// </summary>
#pragma warning disable CS8618
    // ReSharper disable once InconsistentNaming
    public NpgsqlTsVector TitleTSV { get; set; }
#pragma warning restore CS8618
}
