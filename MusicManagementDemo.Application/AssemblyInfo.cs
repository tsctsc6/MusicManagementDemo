using Microsoft.Extensions.DependencyInjection;

namespace MusicManagementDemo.Application;

public static class AssemblyInfo
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services;
    }
}