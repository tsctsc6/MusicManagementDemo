using Microsoft.EntityFrameworkCore;
using MusicManagementDemo.Domain.Entity.Music;

namespace MusicManagementDemo.Abstractions.IDbContext;

public interface IMusicAppDbContext
{
    public DbSet<MusicInfo> MusicInfo { get; }
    public DbSet<MusicList> MusicList { get; }
    public DbSet<MusicInfoMusicListMap> MusicInfoMusicListMap { get; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
