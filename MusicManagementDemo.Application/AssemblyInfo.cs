using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using MusicManagementDemo.Abstractions;
using MusicManagementDemo.Application.JobHandlers;
using MusicManagementDemo.Application.PipelineMediators;

namespace MusicManagementDemo.Application;

public static class AssemblyInfo
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IJobManager, JobManager>();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining(typeof(AssemblyInfo));
            cfg.AddOpenBehavior(typeof(ExceptionHandlingBehavior<,>));
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            cfg.AddOpenBehavior(typeof(LoggingBehaviour<,>));
        });
        services.AddValidatorsFromAssemblyContaining(
            typeof(AssemblyInfo),
            includeInternalTypes: true
        );
        return services;
    }
}
