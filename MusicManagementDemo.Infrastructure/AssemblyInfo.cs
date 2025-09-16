using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MusicManagementDemo.Abstractions;
using MusicManagementDemo.Abstractions.IDbContext;
using MusicManagementDemo.Domain.Entity.Identity;
using MusicManagementDemo.Infrastructure.Database;
using MusicManagementDemo.Infrastructure.JobHandler;
using MusicManagementDemo.Infrastructure.Jwt;
using Serilog;

namespace MusicManagementDemo.Infrastructure;

public static class AssemblyInfo
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services
            .AddHealthChecks()
            .AddDbContextCheck<IdentityAppDbContext>()
            .AddDbContextCheck<MusicAppDbContext>()
            .AddDbContextCheck<ManagementAppDbContext>();

        services.AddDatabase(configuration);

        services
            .AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<IdentityAppDbContext>()
            .AddDefaultTokenProviders();
        services.AddScoped<IdentityDbContext<ApplicationUser>, IdentityAppDbContext>();

        services.AddJwt(configuration);
        services.AddAuthorization();

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

        services.AddJsonOptions();
        services.AddSingleton<IJobManager, JobManager>();
        services.AddSingleton<IJwtManager, JwtManager>();
        return services;
    }

    private static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var connectionString = configuration.GetConnectionString("Default");
        services.AddDbContext<MusicAppDbContext>(options =>
            options
                .UseNpgsql(connectionString)
                .UseSeeding(
                    (d2, _) =>
                    {
                        var d = (MusicAppDbContext)d2;
                        d.Database.ExecuteSql(
                            $"""
CREATE TYPE {Schemas.Music}.{Function.GetMusicInfoInMusicListReturnType} AS (
"Id" UUID,
"Title" character varying(200),
"Artist" character varying(100),
"Album" character varying(100)
);
"""
                        );
                        d.Database.ExecuteSql(
                            $"""
-- 定义函数
CREATE OR REPLACE FUNCTION {Schemas.Music}.{Function.GetMusicInfoInMusicList}(
	music_list_id UUID,
    start_id UUID DEFAULT NULL,
    num_items INTEGER DEFAULT 10,
	is_desc BOOL DEFAULT false
)
RETURNS SETOF music.chain_tuple
LANGUAGE plpgsql
AS $$
DECLARE
    current_id UUID := start_id;
    rec RECORD;
	result music.chain_tuple;
BEGIN
	IF current_id IS NOT NULL THEN
		SELECT m."PrevId", m."NextId" INTO rec
		FROM music."MusicInfoMusicListMap" AS m
		WHERE m."MusicListId" = music_list_id AND m."MusicInfoId" = current_id;
		-- 更新当前 ID 为 NextId
		IF is_desc THEN
			current_id := rec."PrevId";
		ELSE
			current_id := rec."NextId";
		END IF;
	END IF;
    FOR i IN 1..num_items LOOP
		-- 查询当前记录
		IF current_id IS NULL THEN
			IF is_desc THEN
				SELECT mi."Id", mi."Title", mi."Artist", mi."Album", m."PrevId", m."NextId" INTO rec
				FROM music."MusicInfoMusicListMap" AS m
				JOIN music."MusicInfo" AS mi ON m."MusicInfoId" = mi."Id"
				WHERE m."MusicListId" = music_list_id AND m."NextId" is NULL;
			ELSE
				SELECT mi."Id", mi."Title", mi."Artist", mi."Album", m."PrevId", m."NextId" INTO rec
				FROM music."MusicInfoMusicListMap" AS m
				JOIN music."MusicInfo" AS mi ON m."MusicInfoId" = mi."Id"
				WHERE m."MusicListId" = music_list_id AND m."PrevId" is NULL;
			END IF;
		ELSE
			SELECT mi."Id", mi."Title", mi."Artist", mi."Album", m."PrevId", m."NextId" INTO rec
			FROM music."MusicInfoMusicListMap" AS m
			JOIN music."MusicInfo" AS mi ON m."MusicInfoId" = mi."Id"
			WHERE m."MusicListId" = music_list_id AND m."MusicInfoId" = current_id;
        END IF;
        -- 如果未找到，退出循环
		IF NOT FOUND THEN
			EXIT;
		END IF;

		result."Id" := rec."Id";
        result."Title" := rec."Title";
        result."Artist" := rec."Artist";
		result."Album" := rec."Album";
		
        -- 返回当前元组
        RETURN Next result;
        
        -- 更新当前 ID 为 NextId
        IF is_desc THEN
			current_id := rec."PrevId";
		ELSE
			current_id := rec."NextId";
		END IF;
        
        -- 如果 NextId 为 NULL，退出循环
        IF current_id IS NULL THEN
            EXIT;
        END IF;
    END LOOP;
END;
$$;
"""
                        );
                    }
                )
        );
        services.AddDbContext<ManagementAppDbContext>(options =>
            options.UseNpgsql(connectionString)
        );
        services.AddDbContext<IdentityAppDbContext>(options =>
            options
                .UseNpgsql(connectionString)
                .UseSeeding(
                    (d2, _) =>
                    {
                        var d = (IdentityAppDbContext)d2;
                        d.Roles.Add(new() { Name = "Admin", NormalizedName = "ADMIN" });
                        d.SaveChanges();
                    }
                )
        );
        services.AddScoped<IManagementAppDbContext, ManagementAppDbContext>();
        services.AddScoped<IMusicAppDbContext, MusicAppDbContext>();
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
                        if (context.Principal is null)
                        {
                            context.Fail("Unauthorized");
                            return;
                        }

                        var userId = context
                            .Principal.Claims.SingleOrDefault(c =>
                                c.Type == JwtRegisteredClaimNames.Sub
                            )
                            ?.Value;
                        if (string.IsNullOrEmpty(userId))
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

                        var dbContext = context.HttpContext.RequestServices.GetRequiredService<
                            IdentityDbContext<ApplicationUser>
                        >();
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
