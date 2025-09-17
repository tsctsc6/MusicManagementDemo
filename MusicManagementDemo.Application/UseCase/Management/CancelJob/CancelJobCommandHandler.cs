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
        return result switch
        {
            ErrResult<long, string> errResult => ServiceResult.Err(503, [errResult.Value]),
            OkResult<long, string> okResult => ServiceResult.Ok(new { JobId = okResult.Value }),
            _ => ServiceResult.Err(503, ["内部错误"]),
        };
    }
}
