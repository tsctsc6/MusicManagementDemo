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
    public Task<IServiceResult> Handle(
        CancelJobCommand request,
        CancellationToken cancellationToken
    )
    {
        var result = jobManager.CancelJob(request.JobId);
        if (result is ErrResult<long, string> err)
        {
            logger.LogError("{err}", err.Value);
        }
        return result switch
        {
            ErrResult<long, string> errResult => Task.FromResult<IServiceResult>(
                ServiceResult.Err(503, [errResult.Value])
            ),
            OkResult<long, string> okResult => Task.FromResult<IServiceResult>(
                ServiceResult.Ok(new { JobId = okResult.Value })
            ),
            _ => Task.FromResult<IServiceResult>(ServiceResult.Err(503, ["内部错误"])),
        };
    }
}
