using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MusicManagementDemo.Abstractions.IDbContext;
using MusicManagementDemo.Domain.Entity.Identity;
using MusicManagementDemo.Infrastructure.Database.Database;

namespace MusicManagementDemo.Infrastructure.Database;

public static class DependencyInjectionModule
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

    public static IServiceCollection AddDatabaseInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        string connectionStringName
    )
    {
        services.AddHealthChecks().AddDbContextCheck<AppDbContext>();

        services.AddDatabase(
            configuration.GetConnectionString(connectionStringName) ?? string.Empty
        );

        services
            .AddIdentity<ApplicationUser, IdentityRole<Guid>>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        return services;
    }

    private static IServiceCollection AddDatabase(
        this IServiceCollection services,
        string connectionString
    )
    {
        // 可选：AddDbContextPool
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
                )
                .UseAsyncSeeding(
                    async (d2, _, ct) =>
                    {
                        var d = (AppDbContext)d2;
                        await d.Database.ExecuteSqlRawAsync(
                            DbFunctions.DefineGetMusicInfoInMusicListReturnType,
                            ct
                        );
                        await d.Database.ExecuteSqlRawAsync(
                            DbFunctions.DefineGetMusicInfoInMusicList,
                            cancellationToken: ct
                        );
                        await d.Roles.AddAsync(
                            new() { Name = "Admin", NormalizedName = "ADMIN" },
                            ct
                        );
                        await d.SaveChangesAsync(ct);
                    }
                );
            ;
#if DEBUG
            options.EnableSensitiveDataLogging();
#endif
        });
        services.AddScoped<IDbContext, AppDbContext>(sp => sp.GetRequiredService<AppDbContext>());
        services.AddScoped<IIdentityDbContext, AppDbContext>(sp =>
            sp.GetRequiredService<AppDbContext>()
        );
        services.AddScoped<IManagementAppDbContext, AppDbContext>(sp =>
            sp.GetRequiredService<AppDbContext>()
        );
        services.AddScoped<IMusicAppDbContext, AppDbContext>(sp =>
            sp.GetRequiredService<AppDbContext>()
        );

        return services;
    }
}
