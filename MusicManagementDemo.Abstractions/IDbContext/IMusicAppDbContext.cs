using Microsoft.EntityFrameworkCore;
using MusicManagementDemo.Domain.Entity.Music;

namespace MusicManagementDemo.Abstractions.IDbContext;

public interface IMusicAppDbContext : IDbContext
{
    public DbSet<MusicInfo> MusicInfos { get; }
    public DbSet<MusicList> MusicLists { get; }
    public DbSet<MusicInfoMusicListMap> MusicInfoMusicListMaps { get; }

    public IQueryable<MusicInfo> GetMusicInfoInMusicList(
        Guid musicListId,
        Guid? referenceId,
        int pageSize,
        bool asc
    );
}
