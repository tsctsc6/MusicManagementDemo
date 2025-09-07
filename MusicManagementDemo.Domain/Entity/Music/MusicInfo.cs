using NpgsqlTypes;

namespace MusicManagementDemo.Domain.Entity.Music;

public sealed class MusicInfo : SharedKernel.Entity
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Artist { get; set; } = string.Empty;

    public string Album { get; set; } = string.Empty;

    public string FilePath { get; set; } = string.Empty;

    public NpgsqlTsVector TitleTSV { get; set; }
}
