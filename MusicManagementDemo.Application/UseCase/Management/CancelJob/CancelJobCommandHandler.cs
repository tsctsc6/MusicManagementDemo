using Mediator;
using Microsoft.Extensions.Logging;
using MusicManagementDemo.Abstractions;
using MusicManagementDemo.Application.Responses;
using RustSharp;
using static MusicManagementDemo.Application.Responses.ApiResult<MusicManagementDemo.Application.UseCase.Management.CancelJob.CancelJobCommandResponse>;

namespace MusicManagementDemo.Application.UseCase.Management.CancelJob;

internal sealed class CancelJobCommandHandler(
    IJobManager jobManager,
    ILogger<CancelJobCommandHandler> logger
) : IRequestHandler<CancelJobCommand, ApiResult<CancelJobCommandResponse>>
{
    public async ValueTask<ApiResult<CancelJobCommandResponse>> Handle(
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
                return Err(503, errResult.Value);
            case OkResult<long, string> okResult:
                logger.LogInformation("{err}", okResult.Value);
                return Ok(new CancelJobCommandResponse(okResult.Value));
            default:
                logger.LogInformation("Unknown type {@result}", result);
                return Err(503, "内部错误");
        }
    }
}
