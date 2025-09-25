using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MusicManagementDemo.Abstractions.IDbContext;
using Npgsql;

namespace FunctionalTesting;

public class BaseTestingClass : IDisposable
{
    private readonly IServiceProvider _services;
    protected readonly IMediator mediator;

    public BaseTestingClass(IServiceProvider services)
    {
        var config = services.GetRequiredService<IConfiguration>();
        using var conn = new NpgsqlConnection(config["ConnectionStrings:Postgres"]);
        conn.Open();
        new NpgsqlCommand(
            $"CREATE DATABASE {config["DbName"]} WITH TABLESPACE = {config["VirtualTableSpace"]};",
            conn
        ).ExecuteNonQuery();
        using var scope = services.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<IDbContext>();
        dbContext.Database.Migrate();

        _services = services;

        mediator = services.GetRequiredService<IMediator>();
    }

    public void Dispose()
    {
        var config = _services.GetRequiredService<IConfiguration>();
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
