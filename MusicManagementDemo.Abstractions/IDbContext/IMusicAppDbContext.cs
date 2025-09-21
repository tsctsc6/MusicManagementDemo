using Microsoft.EntityFrameworkCore;
using MusicManagementDemo.Domain.Entity.Music;

namespace MusicManagementDemo.Abstractions.IDbContext;

public interface IMusicAppDbContext : IDbContext
{
    public DbSet<MusicInfo> MusicInfo { get; }
    public DbSet<MusicList> MusicList { get; }
    public DbSet<MusicInfoMusicListMap> MusicInfoMusicListMap { get; }
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
