namespace MusicManagementDemo.Domain.Entity.Music;

public class MusicInfoMusicListMap : SharedKernel.Entity
{
    public Guid MusicInfoId { get; set; }

    public Guid MusicListId { get; set; }

    public DateTime CreatedAt { get; set; }
}
