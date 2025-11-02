using FunctionalTesting.Infrastructure.JobHandler;
using FunctionalTesting.Infrastructure.VirtualFileSystem;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using MusicManagementDemo.Abstractions;

namespace FunctionalTesting.Infrastructure;

public static class DependencyInjectionModule
{
    public static IServiceCollection AddTestInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IFileEnumerator, FileEnumerator>();
        services.AddSingleton<IMusicInfoParser, MusicInfoParser>();
        services.AddSingleton<IFileStreamProvider, FileStreamProvider>();

        services.AddSingleton<ILogger, NullLogger>();
        services.AddSingleton(typeof(ILogger<>), typeof(NullLogger<>));

        return services;
    }
}
