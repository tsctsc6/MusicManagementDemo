using MusicManagementDemo.SharedKernel;
using NpgsqlTypes;

namespace MusicManagementDemo.Domain.Music;

public sealed class MusicInfo : Entity
{
    public Guid Id { get; set; }
    
    public string Title { get; set; } = string.Empty;
    
    public string Artist { get; set; } = string.Empty;
    
    public string Album { get; set; } = string.Empty;
    
    public string FilePath { get; set; } = string.Empty;
    
    public NpgsqlTsVector TitleTSV { get; set; }
}
