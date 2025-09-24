using Microsoft.EntityFrameworkCore;
using MusicManagementDemo.Domain.Entity.Music;

namespace MusicManagementDemo.Abstractions.IDbContext;

public interface IMusicAppDbContext : IDbContext
{
    public DbSet<MusicInfo> MusicInfos { get; }
    public DbSet<MusicList> MusicLists { get; }
    public DbSet<MusicInfoMusicListMap> MusicInfoMusicListMaps { get; }
}

public static class MusicAppDbContextExtensions
{
    public static IQueryable<MusicInfo> GetMusicInfoInMusicList(
        this DbSet<MusicInfo> musicInfo,
        Guid musicListId,
        Guid? referenceId,
        int pageSize,
        bool asc
    )
    {
        return musicInfo.FromSqlRaw(
            referenceId is null
                ? $"""
                SELECT * FROM {DbSchemas.Music}.{DbSchemas.GetMusicInfoInMusicList}('{musicListId}'::UUID, NULL::UUID, {pageSize}, {!asc})
                """
                : $"""
                SELECT * FROM {DbSchemas.Music}.{DbSchemas.GetMusicInfoInMusicList}('{musicListId}'::UUID, '{referenceId}'::UUID, {pageSize}, {!asc})
                """
        );
    }
}
