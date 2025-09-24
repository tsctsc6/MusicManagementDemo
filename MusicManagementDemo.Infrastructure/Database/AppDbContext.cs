using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MusicManagementDemo.Abstractions;
using MusicManagementDemo.Abstractions.IDbContext;
using MusicManagementDemo.Domain.Entity.Identity;
using MusicManagementDemo.Domain.Entity.Management;
using MusicManagementDemo.Domain.Entity.Music;
using MusicManagementDemo.Infrastructure.DbConfig.Identity;
using MusicManagementDemo.Infrastructure.DbConfig.Management;
using MusicManagementDemo.Infrastructure.DbConfig.Music;

namespace MusicManagementDemo.Infrastructure.Database;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<ApplicationUser>(options),
        IIdentityDbContext,
        IManagementAppDbContext,
        IMusicAppDbContext
{
    public DbSet<Storage> Storage { get; set; }
    public DbSet<Job> Job { get; set; }

    public DbSet<MusicInfo> MusicInfo { get; set; }
    public DbSet<MusicList> MusicList { get; set; }
    public DbSet<MusicInfoMusicListMap> MusicInfoMusicListMap { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ApplicationUser>().ToTable("Users", DbSchemas.Identity);
        modelBuilder.Entity<IdentityRole>().ToTable("Roles", DbSchemas.Identity);
        modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims", DbSchemas.Identity);
        modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims", DbSchemas.Identity);
        modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins", DbSchemas.Identity);
        modelBuilder.Entity<IdentityUserRole<string>>().ToTable("UserRoles", DbSchemas.Identity);
        modelBuilder.Entity<IdentityUserToken<string>>().ToTable("UserTokens", DbSchemas.Identity);
        modelBuilder.ApplyConfiguration(new ApplicationUserConfiguration());

        modelBuilder.Entity<Storage>().ToTable("Storages", DbSchemas.Management);
        modelBuilder.Entity<Job>().ToTable("Jobs", DbSchemas.Management);
        modelBuilder.ApplyConfiguration(new StorageConfiguration());
        modelBuilder.ApplyConfiguration(
            new JobConfiguration(AssemblyInfo.DefaultJsonSerializerOptions)
        );

        modelBuilder.Entity<MusicInfo>().ToTable("MusicInfos", DbSchemas.Music);
        modelBuilder.Entity<MusicList>().ToTable("MusicLists", DbSchemas.Music);
        modelBuilder
            .Entity<MusicInfoMusicListMap>()
            .ToTable("MusicInfoMusicListMaps", DbSchemas.Music);
        modelBuilder.ApplyConfiguration(new MusicInfoConfiguration());
        modelBuilder.ApplyConfiguration(new MusicListConfiguration());
        modelBuilder.ApplyConfiguration(new MusicInfoMusicListMapConfiguration());
    }
}
