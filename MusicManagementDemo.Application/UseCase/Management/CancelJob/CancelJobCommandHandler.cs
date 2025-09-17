using MediatR;
using Microsoft.Extensions.Logging;
using MusicManagementDemo.Abstractions;
using MusicManagementDemo.Application.Responses;
using RustSharp;

namespace MusicManagementDemo.Application.UseCase.Management.CancelJob;

public class CancelJobCommandHandler(
    IJobManager jobManager,
    ILogger<CancelJobCommandHandler> logger
) : IRequestHandler<CancelJobCommand, IServiceResult>
{
    public async Task<IServiceResult> Handle(
        CancelJobCommand request,
        CancellationToken cancellationToken
    )
    {
        var result = await jobManager.CancelJobAsync(request.JobId);
        if (result is ErrResult<long, string> err)
        {
            logger.LogError("{err}", err.Value);
        }
        switch (result)
        {
            case ErrResult<long, string> errResult:
                logger.LogError("{err}", errResult.Value);
                return ServiceResult.Err(503, [errResult.Value]);
            case OkResult<long, string> okResult:
                logger.LogInformation("{err}", okResult.Value);
                return ServiceResult.Ok(new { JobId = okResult.Value });
            default:
                logger.LogInformation("Unknown type {@result}", result);
                return ServiceResult.Err(503, ["内部错误"]);
        }
    }
}
