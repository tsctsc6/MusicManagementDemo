using MusicManagementDemo.Application;
using MusicManagementDemo.Infrastructure.Core;
using MusicManagementDemo.Infrastructure.Database;
using MusicManagementDemo.WebApi;
using MusicManagementDemo.WebApi.Endpoints;
using Serilog;
using InjectioTags = MusicManagementDemo.WebApi.InjectioTags;

var builder = WebApplication.CreateBuilder(args);

builder
    .Services.AddApplication()
    .AddDatabaseInfrastructure(builder.Configuration, "Default")
    .AddSharedInfrastructure(builder.Configuration)
    .AddRealInfrastructure()
    .AddWebApi(builder.Configuration)
    .AddWebApiByInjectio(InjectioTags.Endpoint);

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
