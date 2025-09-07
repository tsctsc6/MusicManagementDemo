using Microsoft.EntityFrameworkCore;
using MusicManagementDemo.Domain.Entity.Management;

namespace MusicManagementDemo.Infrastructure.Database;

public sealed class ManagementAppDbContext(DbContextOptions<ManagementAppDbContext> options)
    : DbContext(options)
{
    public DbSet<Storage> Storage { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schemas.Management);
        base.OnModelCreating(modelBuilder);
    }
}
