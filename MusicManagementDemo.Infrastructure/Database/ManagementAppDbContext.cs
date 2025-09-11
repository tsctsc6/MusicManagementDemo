using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MusicManagementDemo.Abstractions.IDbContext;
using MusicManagementDemo.Domain.Entity.Management;
using MusicManagementDemo.Infrastructure.DbConfig.Management;

namespace MusicManagementDemo.Infrastructure.Database;

public sealed class ManagementAppDbContext(
    DbContextOptions<ManagementAppDbContext> options,
    IServiceProvider service
) : DbContext(options), IManagementAppDbContext
{
    public DbSet<Storage> Storage { get; set; }

    public DbSet<Job> Job { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schemas.Management);
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new StorageConfiguration());
        modelBuilder.ApplyConfiguration(
            new JobConfiguration(service.GetRequiredKeyedService<JsonSerializerOptions>("default"))
        );
    }
}
