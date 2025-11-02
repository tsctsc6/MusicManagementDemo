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
    protected readonly IServiceProvider _services;
    protected IMediator mediator;

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
        _services = servicesBuilder.BuildServiceProvider();
        mediator = _services.GetRequiredService<IMediator>();
    }

    public async ValueTask InitializeAsync()
    {
        var config = _services.GetRequiredService<IConfigurationRoot>();
        await using var conn = new NpgsqlConnection(config["ConnectionStrings:Postgres"]);
        await conn.OpenAsync();
        await new NpgsqlCommand(
            $"CREATE DATABASE {config["DbName"]} WITH TABLESPACE = {config["VirtualTableSpace"]};",
            conn
        ).ExecuteNonQueryAsync();
        try
        {
            await using var scope = _services.CreateAsyncScope();
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
        var dbContext = _services.GetRequiredService<IDbContext>();
        await dbContext.Database.EnsureDeletedAsync();
        GC.SuppressFinalize(this);
    }
}
