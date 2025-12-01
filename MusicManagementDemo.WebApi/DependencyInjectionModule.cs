using Microsoft.AspNetCore.Cors.Infrastructure;

namespace MusicManagementDemo.WebApi;

public static class DependencyInjectionModule
{
    public static IServiceCollection AddWebApi(
        this IServiceCollection services,
        ConfigurationManager configuration
    )
    {
        // Add services to the container.
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        services.AddOpenApi();

        services.AddCors(options =>
            options.Configure(configuration.GetSection("FromConfiguration"))
        );

        return services;
    }
}

internal static class CorsOptionsExtensions
{
    private static bool AllowAny(IList<string> values) => values.Count == 0 || values[0] == "*";

    public static void Configure(this CorsOptions options, IConfigurationSection section)
    {
        var corsPolicy = section.Get<CorsPolicy>() ?? new CorsPolicy();
        options.AddDefaultPolicy(policy =>
        {
            var headers = corsPolicy.Headers;
            var origins = corsPolicy.Origins;
            var methods = corsPolicy.Methods;

            if (AllowAny(headers))
                policy.AllowAnyHeader();
            else
                policy.WithHeaders([.. headers]);

            if (AllowAny(origins))
                policy.AllowAnyOrigin();
            else
                policy.WithOrigins([.. origins]).AllowCredentials(); // 如果需要发送凭据

            if (AllowAny(methods))
                policy.AllowAnyMethod();
            else
                policy.WithMethods([.. methods]);
        });
    }
}
