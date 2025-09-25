using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MusicManagementDemo.Abstractions;
using MusicManagementDemo.Abstractions.IDbContext;
using MusicManagementDemo.DbInfrastructure.Database;
using MusicManagementDemo.DbInfrastructure.Jwt;
using MusicManagementDemo.Domain.Entity.Identity;

namespace MusicManagementDemo.DbInfrastructure;

public static class AssemblyInfo
{
    public static readonly JsonSerializerOptions DefaultJsonSerializerOptions = new()
    {
        DefaultBufferSize = 1024,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        MaxDepth = 12,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        AllowTrailingCommas = false,
        WriteIndented = false,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    public static IServiceCollection AddDbInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddHealthChecks().AddDbContextCheck<AppDbContext>();

        services.AddDatabase(configuration.GetConnectionString("Default") ?? string.Empty);

        services
            .AddIdentity<ApplicationUser, IdentityRole<Guid>>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        services.AddJwt(configuration);
        services.AddAuthorization();

        services.AddJsonOptions();

        services.AddSingleton<IJwtManager, JwtManager>();

        return services;
    }

    private static IServiceCollection AddDatabase(
        this IServiceCollection services,
        string connectionString
    )
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options
                .UseNpgsql(connectionString)
                .UseSeeding(
                    (d2, _) =>
                    {
                        var d = (AppDbContext)d2;
                        d.Database.ExecuteSqlRaw(
                            DbFunctions.DefineGetMusicInfoInMusicListReturnType
                        );
                        d.Database.ExecuteSqlRaw(DbFunctions.DefineGetMusicInfoInMusicList);
                        d.Roles.Add(new() { Name = "Admin", NormalizedName = "ADMIN" });
                        d.SaveChanges();
                    }
                );
#if DEBUG
            options.EnableSensitiveDataLogging();
#endif
        });
        services.AddScoped<IDbContext, AppDbContext>();
        services.AddScoped<IIdentityDbContext, AppDbContext>();
        services.AddScoped<IManagementAppDbContext, AppDbContext>();
        services.AddScoped<IMusicAppDbContext, AppDbContext>();

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
                    RoleClaimType = AppJwtRegisteredClaimNames.Roles,
                    NameClaimType = JwtRegisteredClaimNames.UniqueName,
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        if (
                            !Guid.TryParse(
                                context
                                    .Principal?.Claims.SingleOrDefault(c =>
                                        c.Type == JwtRegisteredClaimNames.Sub
                                    )
                                    ?.Value,
                                out var userId
                            )
                        )
                        {
                            context.Fail("Unauthorized");
                            return;
                        }

                        var concurrencyStamp = context
                            .Principal.Claims.SingleOrDefault(c =>
                                c.Type == AppJwtRegisteredClaimNames.ConcurrencyStamp
                            )
                            ?.Value;
                        if (string.IsNullOrEmpty(concurrencyStamp))
                        {
                            context.Fail("Unauthorized");
                            return;
                        }

                        var dbContext =
                            context.HttpContext.RequestServices.GetRequiredService<IIdentityDbContext>();
                        var user = await dbContext.Users.SingleOrDefaultAsync(u => u.Id == userId);
                        if (user is null)
                        {
                            context.Fail("Unauthorized");
                            return;
                        }

                        if (user.ConcurrencyStamp != concurrencyStamp)
                        {
                            context.Fail("Unauthorized");
                        }
                    },
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
