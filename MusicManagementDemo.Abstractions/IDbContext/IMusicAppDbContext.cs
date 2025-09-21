using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MusicManagementDemo.Domain.Entity.Music;

namespace MusicManagementDemo.Abstractions.IDbContext;

public interface IMusicAppDbContext : IDbContext
{
    public DbSet<MusicInfo> MusicInfo { get; }
    public DbSet<MusicList> MusicList { get; }
    public DbSet<MusicInfoMusicListMap> MusicInfoMusicListMap { get; }
}
