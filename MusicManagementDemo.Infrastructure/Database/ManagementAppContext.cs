using Microsoft.EntityFrameworkCore;
using MusicManagementDemo.Domain.Entity.Management;

namespace MusicManagementDemo.Infrastructure.Database;

public sealed class ManagementAppContext(DbContextOptions<ManagementAppContext> options)
    : DbContext(options)
{
    public DbSet<Storage> Storage { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ManagementAppContext).Assembly);

        modelBuilder.HasDefaultSchema(Schemas.Management);
    }
}