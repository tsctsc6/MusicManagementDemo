using Microsoft.EntityFrameworkCore;
using MusicManagementDemo.Domain.Entity.Music;

namespace MusicManagementDemo.Infrastructure.Database;

public sealed class MusicAppDbContext(DbContextOptions<MusicAppDbContext> options)
    : DbContext(options)
{
    public DbSet<MusicInfo> MusicInfo { get; set; }
    public DbSet<MusicList> MusicList { get; set; }
    public DbSet<MusicInfoMusicListMap> MusicInfoMusicListMap { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MusicAppDbContext).Assembly);

        modelBuilder.HasDefaultSchema(Schemas.Music);
    }
}
