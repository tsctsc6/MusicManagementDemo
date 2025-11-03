using MusicManagementDemo.AppInfrastructure;
using MusicManagementDemo.Application;
using MusicManagementDemo.DbInfrastructure;
using MusicManagementDemo.WebApi.Endpoints;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder
    .Services.AddApplication()
    .AddDbInfrastructure(builder.Configuration)
    .AddAppInfrastructure()
    .AddMusicManagementDemoWebApi();

//builder.Services.AddEndpoints(Assembly.GetExecutingAssembly());

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
