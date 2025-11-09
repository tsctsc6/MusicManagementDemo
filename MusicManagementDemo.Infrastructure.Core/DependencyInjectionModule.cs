using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MusicManagementDemo.Abstractions;
using MusicManagementDemo.Abstractions.IDbContext;
using MusicManagementDemo.Infrastructure.Core.JobHandler;
using MusicManagementDemo.Infrastructure.Core.Jwt;
using Serilog;

namespace MusicManagementDemo.Infrastructure.Core;

public static class DependencyInjectionModule
{
    public static IServiceCollection AddSharedInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddJwt(configuration);
        services.AddAuthorization();
        services.AddSingleton<IJwtManager, JwtManager>();

        services.AddJsonOptions();

        return services;
    }

    public static IServiceCollection AddRealInfrastructure(
        this IServiceCollection services
    )
    {
        services.AddSingleton<IFileEnumerator, FileEnumerator>();
        services.AddSingleton<IMusicInfoParser, MusicInfoParser>();
        services.AddSingleton<IFileStreamProvider, FileStreamProvider>();

        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File(
                path: Path.Combine(
                    Path.GetDirectoryName(Environment.ProcessPath)!,
                    "logs",
                    "log-.txt"
                ),
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7,
                rollOnFileSizeLimit: true,
                fileSizeLimitBytes: 10000000
            )
            .CreateLogger();
        services.AddSerilog();

        return services;
    }

    private static IServiceCollection AddJwt(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = configuration["Jwt:Audience"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)
                    ),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromHours(1),
                    RoleClaimType = AppJwtRegisteredClaimNames.Roles,
                    NameClaimType = JwtRegisteredClaimNames.UniqueName,
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        if (
                            !Guid.TryParse(
                                context
                                    .Principal?.Claims.SingleOrDefault(c =>
                                        c.Type == JwtRegisteredClaimNames.Sub
                                    )
                                    ?.Value,
                                out var userId
                            )
                        )
                        {
                            context.Fail("Unauthorized");
                            return;
                        }

                        var concurrencyStamp = context
                            .Principal.Claims.SingleOrDefault(c =>
                                c.Type == AppJwtRegisteredClaimNames.ConcurrencyStamp
                            )
                            ?.Value;
                        if (string.IsNullOrEmpty(concurrencyStamp))
                        {
                            context.Fail("Unauthorized");
                            return;
                        }

                        var dbContext =
                            context.HttpContext.RequestServices.GetRequiredService<IIdentityDbContext>();
                        var user = await dbContext.Users.SingleOrDefaultAsync(u => u.Id == userId);
                        if (user is null)
                        {
                            context.Fail("Unauthorized");
                            return;
                        }

                        if (user.ConcurrencyStamp != concurrencyStamp)
                        {
                            context.Fail("Unauthorized");
                        }
                    },
                };
            });
        return services;
    }

    private static IServiceCollection AddJsonOptions(this IServiceCollection services)
    {
        services.Configure<JsonOptions>(op =>
        {
            op.SerializerOptions.DefaultBufferSize = 1024;
            op.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            op.SerializerOptions.MaxDepth = 12;
            op.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            op.SerializerOptions.AllowTrailingCommas = false;
            op.SerializerOptions.WriteIndented = false;
            op.SerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
        });

        services.AddKeyedSingleton<JsonSerializerOptions>(
            "default",
            (_, _) =>
                new JsonSerializerOptions
                {
                    DefaultBufferSize = 1024,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                    MaxDepth = 12,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    AllowTrailingCommas = false,
                    WriteIndented = false,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                }
        );
        return services;
    }
}
