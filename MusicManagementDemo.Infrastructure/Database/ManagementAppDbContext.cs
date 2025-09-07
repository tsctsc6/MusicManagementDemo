using Microsoft.EntityFrameworkCore;
using MusicManagementDemo.Domain.Entity.Management;

namespace MusicManagementDemo.Infrastructure.Database;

public sealed class ManagementAppDbContext(DbContextOptions<ManagementAppDbContext> options)
    : DbContext(options)
{
    public DbSet<Storage> Storage { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ManagementAppDbContext).Assembly);

        modelBuilder.HasDefaultSchema(Schemas.Management);
    }
}
