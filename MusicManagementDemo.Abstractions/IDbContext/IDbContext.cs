using Microsoft.EntityFrameworkCore.Infrastructure;

namespace MusicManagementDemo.Abstractions.IDbContext;

public interface IDbContext : IDisposable, IAsyncDisposable
{
    public DatabaseFacade Database { get; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}