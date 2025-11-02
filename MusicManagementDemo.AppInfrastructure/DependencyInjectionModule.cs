using Microsoft.Extensions.DependencyInjection;
using MusicManagementDemo.Abstractions;
using MusicManagementDemo.AppInfrastructure.JobHandler;
using Serilog;

namespace MusicManagementDemo.AppInfrastructure;

public static class DependencyInjectionModule
{
    public static IServiceCollection AddAppInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IFileEnumerator, FileEnumerator>();
        services.AddSingleton<IMusicInfoParser, MusicInfoParser>();
        services.AddSingleton<IFileStreamProvider, FileStreamProvider>();

        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File(
                path: Path.Combine(
                    Path.GetDirectoryName(Environment.ProcessPath)!,
                    "logs",
                    "log-.txt"
                ),
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7,
                rollOnFileSizeLimit: true,
                fileSizeLimitBytes: 10000000
            )
            .CreateLogger();
        services.AddSerilog();

        return services;
    }
}
