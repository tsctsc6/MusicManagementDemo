using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace MusicManagementDemo.Application;

public static class AssemblyInfo
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining(typeof(AssemblyInfo));
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });
        services.AddValidatorsFromAssemblyContaining(typeof(AssemblyInfo), includeInternalTypes: true);
        return services;
    }
}
