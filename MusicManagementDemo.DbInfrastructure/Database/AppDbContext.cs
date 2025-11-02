using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MusicManagementDemo.Abstractions;
using MusicManagementDemo.Abstractions.IDbContext;
using MusicManagementDemo.DbInfrastructure.DbConfig.Identity;
using MusicManagementDemo.DbInfrastructure.DbConfig.Management;
using MusicManagementDemo.DbInfrastructure.DbConfig.Music;
using MusicManagementDemo.Domain.Entity.Identity;
using MusicManagementDemo.Domain.Entity.Management;
using MusicManagementDemo.Domain.Entity.Music;

namespace MusicManagementDemo.DbInfrastructure.Database;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options),
        IIdentityDbContext,
        IManagementAppDbContext,
        IMusicAppDbContext
{
    public DbSet<Storage> Storages { get; set; }
    public DbSet<Job> Jobs { get; set; }

    public DbSet<MusicInfo> MusicInfos { get; set; }
    public DbSet<MusicList> MusicLists { get; set; }
    public DbSet<MusicInfoMusicListMap> MusicInfoMusicListMaps { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ApplicationUser>().ToTable("Users", DbSchemas.Identity);
        modelBuilder.Entity<IdentityRole<Guid>>().ToTable("Roles", DbSchemas.Identity);
        modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims", DbSchemas.Identity);
        modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims", DbSchemas.Identity);
        modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins", DbSchemas.Identity);
        modelBuilder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles", DbSchemas.Identity);
        modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens", DbSchemas.Identity);
        modelBuilder.ApplyConfiguration(new ApplicationUserConfiguration());

        modelBuilder.Entity<Storage>().ToTable($"{nameof(Storage)}s", DbSchemas.Management);
        modelBuilder.Entity<Job>().ToTable($"{nameof(Job)}s", DbSchemas.Management);
        modelBuilder.ApplyConfiguration(new StorageConfiguration());
        modelBuilder.ApplyConfiguration(
            new JobConfiguration(DependencyInjectionModule.DefaultJsonSerializerOptions)
        );

        modelBuilder.Entity<MusicInfo>().ToTable($"{nameof(MusicInfo)}s", DbSchemas.Music);
        modelBuilder.Entity<MusicList>().ToTable($"{nameof(MusicList)}s", DbSchemas.Music);
        modelBuilder
            .Entity<MusicInfoMusicListMap>()
            .ToTable($"{nameof(MusicInfoMusicListMap)}s", DbSchemas.Music);
        modelBuilder.ApplyConfiguration(new MusicInfoConfiguration());
        modelBuilder.ApplyConfiguration(new MusicListConfiguration());
        modelBuilder.ApplyConfiguration(new MusicInfoMusicListMapConfiguration());
    }

    public IQueryable<MusicInfo> GetMusicInfoInMusicList(
        Guid musicListId,
        Guid? referenceId,
        int pageSize,
        bool asc
    )
    {
        return MusicInfos.FromSqlRaw(
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
