using Microsoft.Extensions.DependencyInjection;

namespace MusicManagementDemo.Domain;

public static class AssemblyInfo
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        return services;
    }
}
