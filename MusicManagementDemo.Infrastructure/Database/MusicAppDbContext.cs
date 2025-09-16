using Microsoft.EntityFrameworkCore;
using MusicManagementDemo.Abstractions;
using MusicManagementDemo.Abstractions.IDbContext;
using MusicManagementDemo.Domain.Entity.Music;
using MusicManagementDemo.Infrastructure.DbConfig.Music;

namespace MusicManagementDemo.Infrastructure.Database;

public sealed class MusicAppDbContext(DbContextOptions<MusicAppDbContext> options)
    : DbContext(options),
        IMusicAppDbContext
{
    public DbSet<MusicInfo> MusicInfo { get; set; }
    public DbSet<MusicList> MusicList { get; set; }
    public DbSet<MusicInfoMusicListMap> MusicInfoMusicListMap { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(DbSchemas.Music);
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new MusicInfoConfiguration());
        modelBuilder.ApplyConfiguration(new MusicListConfiguration());
        modelBuilder.ApplyConfiguration(new MusicInfoMusicListMapConfiguration());
    }
}
