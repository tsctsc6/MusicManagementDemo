using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using MusicManagementDemo.Abstractions;
using MusicManagementDemo.Application.JobHandlers;
using MusicManagementDemo.Application.PipelineMediators;

namespace MusicManagementDemo.Application;

public static class DependencyInjectionModule
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IJobManager, JobManager>();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining(typeof(DependencyInjectionModule));
            cfg.AddOpenBehavior(typeof(ExceptionHandlingBehavior<,>));
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            cfg.AddOpenBehavior(typeof(LoggingBehaviour<,>));
        });
        services.AddValidatorsFromAssemblyContaining(
            typeof(DependencyInjectionModule),
            includeInternalTypes: true
        );
        return services;
    }
}
