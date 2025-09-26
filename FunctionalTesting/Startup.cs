using FunctionalTesting.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MusicManagementDemo.Application;
using MusicManagementDemo.DbInfrastructure;

namespace FunctionalTesting;

public static class Startup
{
    /// <summary>
    /// 在 appsettings.Testing.json 中， ConnectionStrings:Default 中的连接字符串，不要指定 Databse ，
    /// 程序会自动加上一个随机的数据库名称。
    /// </summary>
    /// <returns></returns>
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
        servicesBuilder.AddSingleton<IConfiguration>(_ => newConfiguration);
        return servicesBuilder.BuildServiceProvider();
    }
}
