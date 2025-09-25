using FunctionalTesting.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MusicManagementDemo.Application;
using MusicManagementDemo.DbInfrastructure;
using Serilog;

namespace FunctionalTesting;

public static class Startup
{
    public static IServiceProvider ConfigureMyServices()
    {
        IServiceCollection servicesBuilder = new ServiceCollection();
        var oldConfiguration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Testing.json", optional: false, reloadOnChange: true)
            .Build();

        var guid = Guid.NewGuid().ToString().Replace('-', '_');
        var oldConnectionString = oldConfiguration.GetConnectionString("Default");

        var newConfiguration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Testing.json", optional: false, reloadOnChange: true)
            .AddInMemoryCollection(
                [
                    new("DbName", $"functional_testing_{guid}"),
                    new(
                        "ConnectionStrings:Default",
                        $"{oldConnectionString}Database=functional_testing_{guid};"
                    ),
                ]
            )
            .Build();

        servicesBuilder
            .AddDbInfrastructure(newConfiguration)
            .AddTestInfrastructure()
            .AddApplication();
        servicesBuilder.AddSingleton(_ => newConfiguration);
        return servicesBuilder.BuildServiceProvider();
    }
}
