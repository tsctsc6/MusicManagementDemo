using MusicManagementDemo.Application;
using MusicManagementDemo.Infrastructure.Core;
using MusicManagementDemo.Infrastructure.Database;
using MusicManagementDemo.WebApi.Endpoints;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder
    .Services.AddApplication()
    .AddDatabaseInfrastructure(builder.Configuration)
    .AddSharedInfrastructure(builder.Configuration)
    .AddRealInfrastructure()
    .AddWebApiByInjectio("Endpoint");

var app = builder.Build();

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
