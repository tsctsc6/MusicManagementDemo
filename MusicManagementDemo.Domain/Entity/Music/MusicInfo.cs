using NpgsqlTypes;

namespace MusicManagementDemo.Domain.Entity.Music;

public sealed class MusicInfo
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Artist { get; set; } = string.Empty;

    public string Album { get; set; } = string.Empty;

    /// <summary>
    /// 相对于存储路径的相对路径
    /// </summary>
    public string FilePath { get; set; } = string.Empty;

    public int StorageId { get; set; }

#pragma warning disable CS8618
    // ReSharper disable once InconsistentNaming
    public NpgsqlTsVector TitleTSV { get; set; }
#pragma warning restore CS8618
}
