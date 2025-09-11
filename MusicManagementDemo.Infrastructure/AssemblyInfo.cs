using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MusicManagementDemo.Abstractions;
using MusicManagementDemo.Abstractions.IDbContext;
using MusicManagementDemo.Domain.Entity.Identity;
using MusicManagementDemo.Infrastructure.Database;
using MusicManagementDemo.Infrastructure.JobHandler;

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
            .AddDbContextCheck<MusicAppDbContext>()
            .AddDbContextCheck<ManagementAppDbContext>();
        services.AddDatabase(configuration);
        services
            .AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<IdentityAppDbContext>()
            .AddDefaultTokenProviders();
        services.AddScoped<IdentityDbContext<ApplicationUser>, IdentityAppDbContext>();
        services.AddJwt(configuration);
        services.AddAuthorization();
        services.AddJsonOptions();
        services.AddSingleton<IJobManager, JobManager>();
        services.AddSingleton<IJwtManager, JwtManager>();
        return services;
    }

    private static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var connectionString = configuration.GetConnectionString("Default");
        services.AddDbContext<MusicAppDbContext>(options => options.UseNpgsql(connectionString));
        services.AddDbContext<ManagementAppDbContext>(options =>
            options.UseNpgsql(connectionString)
        );
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
        services.AddScoped<IManagementAppDbContext, ManagementAppDbContext>();
        services.AddScoped<IMusicAppDbContext, MusicAppDbContext>();
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
                options.MapInboundClaims = false;
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
                    ClockSkew = TimeSpan.FromHours(1),
                    RoleClaimType = "role",
                    NameClaimType = JwtRegisteredClaimNames.UniqueName,
                };
            });
        return services;
    }

    private static IServiceCollection AddJsonOptions(this IServiceCollection services)
    {
        services.Configure<JsonOptions>(op =>
        {
            op.SerializerOptions.DefaultBufferSize = 1024;
            op.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            op.SerializerOptions.MaxDepth = 12;
            op.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            op.SerializerOptions.AllowTrailingCommas = false;
            op.SerializerOptions.WriteIndented = false;
            op.SerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
        });

        services.AddKeyedSingleton<JsonSerializerOptions>(
            "default",
            (_, _) =>
                new JsonSerializerOptions
                {
                    DefaultBufferSize = 1024,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                    MaxDepth = 12,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    AllowTrailingCommas = false,
                    WriteIndented = false,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                }
        );
        return services;
    }
}
