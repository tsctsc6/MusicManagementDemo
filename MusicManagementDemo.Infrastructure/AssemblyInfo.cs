using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MusicManagementDemo.Domain.Identity;
using MusicManagementDemo.Infrastructure.Database;

namespace MusicManagementDemo.Infrastructure;

public static class AssemblyInfo
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services
            .AddHealthChecks()
            .AddDbContextCheck<IdentityAppDbContext>()
            .AddDbContextCheck<MusicAppDbContext>();
        services.AddDatabase(configuration);
        services.AddJwt(configuration);
        services.AddAuthentication();
        services
            .AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<IdentityAppDbContext>()
            .AddDefaultTokenProviders();
        return services;
    }

    private static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var connectionString = configuration.GetConnectionString("Default");
        services.AddDbContext<MusicAppDbContext>(options => options.UseNpgsql(connectionString));
        services.AddDbContext<IdentityAppDbContext>(options =>
            options
                .UseNpgsql(connectionString)
                .UseSeeding(
                    (d2, _) =>
                    {
                        var d = (IdentityAppDbContext)d2;
                        d.Roles.Add(new() { Name = "Admin", NormalizedName = "ADMIN" });
                        d.SaveChanges();
                    }
                )
        );
        return services;
    }

    private static IServiceCollection AddJwt(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = configuration["Jwt:Audience"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)
                    ),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                };
            });
        return services;
    }
}
