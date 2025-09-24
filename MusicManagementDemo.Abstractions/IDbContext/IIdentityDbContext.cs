using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MusicManagementDemo.Domain.Entity.Identity;

namespace MusicManagementDemo.Abstractions.IDbContext;

public interface IIdentityDbContext : IDbContext
{
    public DbSet<ApplicationUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityRoleClaim<string>> RoleClaims { get; set; }
    public DbSet<IdentityUserClaim<string>> UserClaims { get; set; }
    public DbSet<IdentityUserLogin<string>> UserLogins { get; set; }
    public DbSet<IdentityUserRole<string>> UserRoles { get; set; }
    public DbSet<IdentityUserToken<string>> UserTokens { get; set; }
}
