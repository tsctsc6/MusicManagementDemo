using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MusicManagementDemo.Domain.Entity.Identity;

namespace MusicManagementDemo.Abstractions.IDbContext;

public interface IIdentityDbContext : IDbContext
{
    public DbSet<ApplicationUser> Users { get; set; }
    public DbSet<IdentityRole<Guid>> Roles { get; set; }
    public DbSet<IdentityRoleClaim<Guid>> RoleClaims { get; set; }
    public DbSet<IdentityUserClaim<Guid>> UserClaims { get; set; }
    public DbSet<IdentityUserLogin<Guid>> UserLogins { get; set; }
    public DbSet<IdentityUserRole<Guid>> UserRoles { get; set; }
    public DbSet<IdentityUserToken<Guid>> UserTokens { get; set; }
}
