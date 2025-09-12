using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MusicManagementDemo.Domain.Entity.Management;

namespace MusicManagementDemo.Abstractions.IDbContext;

public interface IManagementAppDbContext
{
    public DbSet<Storage> Storage { get; }
    public DbSet<Job> Job { get; }

    public DatabaseFacade Database { get; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
