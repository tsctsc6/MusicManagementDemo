using Microsoft.EntityFrameworkCore;
using MusicManagementDemo.Domain.Music;

namespace MusicManagementDemo.Infrastructure.Database;

public sealed class MusicAppDbContext(DbContextOptions<MusicAppDbContext> options) : DbContext(options)
{
    public DbSet<MusicInfo> MusicInfo { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MusicAppDbContext).Assembly);

        modelBuilder.HasDefaultSchema(Schemas.Music);
    }
}