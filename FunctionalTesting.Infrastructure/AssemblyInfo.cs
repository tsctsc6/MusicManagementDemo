using FunctionalTesting.Infrastructure.JobHandler;
using Microsoft.Extensions.DependencyInjection;
using MusicManagementDemo.Abstractions;

namespace FunctionalTesting.Infrastructure;

public static class AssemblyInfo
{
    public static IServiceCollection AddTestInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IFileEnumerator, FileEnumerator>();
        services.AddSingleton<IMusicInfoParser, MusicInfoParser>();
        return services;
    }
}