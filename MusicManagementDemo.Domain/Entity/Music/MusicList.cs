namespace MusicManagementDemo.Domain.Entity.Music;

public class MusicList : SharedKernel.Entity
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string UserId { get; set; } = Guid.Empty.ToString();

    public DateTime CreatedAt { get; set; }
}
