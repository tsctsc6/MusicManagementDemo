using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MusicManagementDemo.Abstractions.IDbContext;
using Npgsql;

namespace FunctionalTesting;

public class BaseTestingClass : IDisposable
{
    protected readonly IServiceProvider _services;
    protected readonly IMediator mediator;

    public BaseTestingClass()
    {
        _services = Startup.ConfigureMyServices();
        var config = _services.GetRequiredService<IConfigurationRoot>();
        using var conn = new NpgsqlConnection(config["ConnectionStrings:Postgres"]);
        conn.Open();
        new NpgsqlCommand(
            $"CREATE DATABASE {config["DbName"]} WITH TABLESPACE = {config["VirtualTableSpace"]};",
            conn
        ).ExecuteNonQuery();
        try
        {
            using var scope = _services.CreateScope();
            using var dbContext = scope.ServiceProvider.GetRequiredService<IDbContext>();
            dbContext.Database.Migrate();

            mediator = _services.GetRequiredService<IMediator>();
            var x = _services.GetRequiredService<ILogger<BaseTestingClass>>();
        }
        catch (Exception)
        {
            Dispose();
            throw;
        }
    }

    public void Dispose()
    {
        var config = _services.GetRequiredService<IConfigurationRoot>();
        using var conn = new NpgsqlConnection(config["ConnectionStrings:Postgres"]);
        conn.Open();
        new NpgsqlCommand(
            $"UPDATE pg_database SET datallowconn = 'false' WHERE datname = '{config["DbName"]}';",
            conn
        ).ExecuteNonQuery();
        new NpgsqlCommand(
            $"SELECT pg_terminate_backend(pid) FROM pg_stat_activity WHERE datname = '{config["DbName"]}';",
            conn
        ).ExecuteNonQuery();
        new NpgsqlCommand($"DROP DATABASE {config["DbName"]};", conn).ExecuteNonQuery();
        GC.SuppressFinalize(this);
    }
}
