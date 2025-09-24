using FunctionalTesting.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MusicManagementDemo.Application;
using MusicManagementDemo.DbInfrastructure;

namespace FunctionalTesting;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        var configurationBuilder = new ConfigurationBuilder();
        configurationBuilder.AddJsonFile(
            "appsettings.Testing.json",
            optional: false,
            reloadOnChange: true
        );
        var configuration = configurationBuilder.Build();
        services.AddDbInfrastructure(configuration).AddTestInfrastructure().AddApplication();
    }
}
