using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MusicManagementDemo.Infrastructure.Database;

namespace MusicManagementDemo.Infrastructure;

public static class AssemblyInfo
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddHealthChecks();
        services.AddDatabase(configuration);
        return services;
    }

    private static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var connectionString = configuration.GetConnectionString("Default");
        services.AddDbContext<MusicAppDbContext>(options => options.UseNpgsql(connectionString));
        services.AddDbContext<IdentityAppDbContext>(options => options.UseNpgsql(connectionString));
        return services;
    }
}
