using Microsoft.Extensions.DependencyInjection;
using MusicManagementDemo.Abstractions;
using MusicManagementDemo.AppInfrastructure.JobHandler;

namespace MusicManagementDemo.AppInfrastructure;

public static class AssemblyInfo
{
    public static IServiceCollection AddAppInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IFileEnumerator, FileEnumerator>();
        services.AddSingleton<IMusicInfoParser, MusicInfoParser>();
        services.AddSingleton<IFileStreamProvider, FileStreamProvider>();
        return services;
    }
}
