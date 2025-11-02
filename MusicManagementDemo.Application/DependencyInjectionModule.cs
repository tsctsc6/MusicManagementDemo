using FluentValidation;
using Mediator;
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
        services.AddMediator(options =>
        {
            options.Namespace = "MusicManagementDemo.Mediator";
            options.ServiceLifetime = ServiceLifetime.Scoped;
            options.GenerateTypesAsInternal = true;
            options.NotificationPublisherType = typeof(ForeachAwaitPublisher);
            options.Assemblies = [typeof(DependencyInjectionModule)];
            options.PipelineBehaviors =
            [
                typeof(ExceptionHandlingBehavior<,>),
                typeof(ValidationBehavior<,>),
                typeof(LoggingBehaviour<,>),
            ];
            options.StreamPipelineBehaviors = [];
        });
        services.AddValidatorsFromAssemblyContaining(
            typeof(DependencyInjectionModule),
            includeInternalTypes: true
        );
        return services;
    }
}
