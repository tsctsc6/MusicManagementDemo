namespace MusicManagementDemo.Domain.Entity.Music;

public class MusicInfoMusicListMap
{
    public Guid MusicInfoId { get; set; }

    public Guid MusicListId { get; set; }

    public Guid? PrevId { get; set; }

    public Guid? NextId { get; set; }

    public DateTime CreatedAt { get; set; }
}
