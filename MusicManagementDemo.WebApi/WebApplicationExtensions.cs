using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace MusicManagementDemo.WebApi;

public static class WebApplicationExtensions
{
    /// <summary>
    /// 健康检查端点：/health/live（存活探针）和 /health/ready（就绪探针）
    /// </summary>
    /// <param name="app"></param>
    public static void MapMyHealthChecks(this IEndpointRouteBuilder app)
    {
        app.MapHealthChecks(
            "/health/live",
            new HealthCheckOptions
            {
                // liveness：不执行任何检查，只要进程活着就 200
                Predicate = _ => false,
            }
        );
        app.MapHealthChecks(
            "/health/ready",
            new HealthCheckOptions
            {
                // readiness：执行已注册的检查
                Predicate = _ => true,
            }
        );
    }
}
