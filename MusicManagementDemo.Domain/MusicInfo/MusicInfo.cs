using MusicManagementDemo.SharedKernel;

namespace MusicManagementDemo.Domain.MusicInfo;

public sealed class MusicInfo : Entity
{
    private Guid Id { get; set; }
    
    private string Title { get; set; } = string.Empty;
    
    private string Artist { get; set; } = string.Empty;
    
    private string Album { get; set; } = string.Empty;
    
    private string FilePath { get; set; } = string.Empty;
}
