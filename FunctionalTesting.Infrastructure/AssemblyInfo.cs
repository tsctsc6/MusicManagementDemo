using FunctionalTesting.Infrastructure.JobHandler;
using FunctionalTesting.Infrastructure.VirtualFileSystem;
using Microsoft.Extensions.DependencyInjection;
using MusicManagementDemo.Abstractions;

namespace FunctionalTesting.Infrastructure;

public static class AssemblyInfo
{
    public static IServiceCollection AddTestInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IFileEnumerator, FileEnumerator>();
        services.AddSingleton<IMusicInfoParser, MusicInfoParser>();
        services.AddSingleton<IFileStreamProvider, FileStreamProvider>();
        return services;
    }
}
