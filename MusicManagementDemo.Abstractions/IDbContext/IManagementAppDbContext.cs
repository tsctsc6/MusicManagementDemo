using Microsoft.EntityFrameworkCore;
using MusicManagementDemo.Domain.Entity.Management;

namespace MusicManagementDemo.Abstractions.IDbContext;

public interface IManagementAppDbContext : IDbContext
{
    public DbSet<Storage> Storage { get; }
    public DbSet<Job> Job { get; }
}
