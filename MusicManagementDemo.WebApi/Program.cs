using Microsoft.AspNetCore.Cors.Infrastructure;
using MusicManagementDemo.Application;
using MusicManagementDemo.Infrastructure.Core;
using MusicManagementDemo.Infrastructure.Database;
using MusicManagementDemo.WebApi.Endpoints;
using Serilog;
using InjectioTags = MusicManagementDemo.WebApi.InjectioTags;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder
    .Services.AddApplication()
    .AddDatabaseInfrastructure(builder.Configuration)
    .AddSharedInfrastructure(builder.Configuration)
    .AddRealInfrastructure()
    .AddWebApiByInjectio(InjectioTags.Endpoint);

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "FromConfiguration",
        policy =>
        {
            var corsConfiguration =
                builder.Configuration.GetSection("CorsPolicy").Get<CorsPolicy>()
                ?? new CorsPolicy();
            if (corsConfiguration.Origins.Contains("*"))
            {
                policy.AllowAnyOrigin();
            }
            else
            {
                policy.WithOrigins([.. corsConfiguration.Origins]);
            }

            if (corsConfiguration.Methods.Contains("*"))
            {
                policy.AllowAnyMethod();
            }
            else
            {
                policy.WithMethods([.. corsConfiguration.Methods]);
            }

            if (corsConfiguration.Headers.Contains("*"))
            {
                policy.AllowAnyHeader();
            }
            else
            {
                policy.WithHeaders([.. corsConfiguration.Headers]);
            }
        }
    );
});

var app = builder.Build();

// 全局异常处理
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error");
}

app.UseCors("FromConfiguration");

app.UseAuthentication();
app.UseAuthorization();

app.MapEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapHealthChecks("/health");

app.UseSerilogRequestLogging();

app.Run();
