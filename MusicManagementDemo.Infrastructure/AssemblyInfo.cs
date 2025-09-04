using Microsoft.Extensions.DependencyInjection;

namespace MusicManagementDemo.Infrastructure;

public static class AssemblyInfo
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddHealthChecks();
        return services;
    }
}
