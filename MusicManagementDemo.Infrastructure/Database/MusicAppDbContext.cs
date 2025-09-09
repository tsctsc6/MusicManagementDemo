using Microsoft.EntityFrameworkCore;
using MusicManagementDemo.Domain.Entity.Music;
using MusicManagementDemo.Infrastructure.DbConfig.Music;

namespace MusicManagementDemo.Infrastructure.Database;

public sealed class MusicAppDbContext(DbContextOptions<MusicAppDbContext> options)
    : DbContext(options)
{
    public DbSet<MusicInfo> MusicInfo { get; set; }
    public DbSet<MusicList> MusicList { get; set; }
    public DbSet<MusicInfoMusicListMap> MusicInfoMusicListMap { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schemas.Music);
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new MusicInfoConfiguration());
        modelBuilder.ApplyConfiguration(new MusicListConfiguration());
        modelBuilder.ApplyConfiguration(new MusicInfoMusicListMapConfiguration());
    }
}
