namespace MusicManagementDemo.Domain.Entity.Management;

public class Storage : SharedKernel.Entity
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Path { get; set; } = string.Empty;
}
