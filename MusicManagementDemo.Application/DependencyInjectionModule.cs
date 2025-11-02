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
            // 不知道为什么，如果指定要扫描的 Assembly ， INotificationHandler 不会出现在 Mediator.g.cs 中
            // 新建最小项目做实验，无法复现此问题。
            // options.Assemblies = [typeof(DependencyInjectionModule).Assembly];
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
