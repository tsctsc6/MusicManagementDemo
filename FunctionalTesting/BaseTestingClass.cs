using FunctionalTesting.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MusicManagementDemo.Abstractions.IDbContext;
using MusicManagementDemo.Application;
using MusicManagementDemo.DbInfrastructure;
using Npgsql;

namespace FunctionalTesting;

public class BaseTestingClass : IAsyncLifetime
{
    protected readonly IServiceProvider ServiceProvider;
    protected readonly IMediator Mediator;

    protected BaseTestingClass()
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
        ServiceProvider = servicesBuilder.BuildServiceProvider();
        Mediator = ServiceProvider.GetRequiredService<IMediator>();
    }

    public async ValueTask InitializeAsync()
    {
        var config = ServiceProvider.GetRequiredService<IConfigurationRoot>();
        await using var conn = new NpgsqlConnection(config["ConnectionStrings:Postgres"]);
        await conn.OpenAsync();
        await new NpgsqlCommand(
            $"CREATE DATABASE {config["DbName"]} WITH TABLESPACE = {config["VirtualTableSpace"]};",
            conn
        ).ExecuteNonQueryAsync();
        try
        {
            await using var scope = ServiceProvider.CreateAsyncScope();
            await using var dbContext = scope.ServiceProvider.GetRequiredService<IDbContext>();
            await dbContext.Database.MigrateAsync();
        }
        catch (Exception)
        {
            await DisposeAsync();
            throw;
        }
    }

    public async ValueTask DisposeAsync()
    {
        await using var scope = ServiceProvider.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IDbContext>();
        await dbContext.Database.EnsureDeletedAsync();
        GC.SuppressFinalize(this);
    }
}
