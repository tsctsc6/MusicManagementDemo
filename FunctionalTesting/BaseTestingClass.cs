using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MusicManagementDemo.Abstractions.IDbContext;
using Npgsql;

namespace FunctionalTesting;

public class BaseTestingClass : IDisposable
{
    private IServiceProvider _services;

    public BaseTestingClass(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<IDbContext>();
        var configuration = services.GetRequiredService<IConfiguration>();
        using var conn = new NpgsqlConnection(configuration["ConnectionStrings:Postgres"]);
        conn.Open();
        new NpgsqlCommand(
            $"CREATE DATABASE {configuration["DbName"]} WITH TABLESPACE = {configuration["VirtualTableSpace"]};",
            conn
        ).ExecuteNonQuery();
        dbContext.Database.Migrate();
        _services = services;
    }

    public void Dispose()
    {
        using var scope = _services.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<IDbContext>();
        dbContext.Database.EnsureDeleted();
    }
}
