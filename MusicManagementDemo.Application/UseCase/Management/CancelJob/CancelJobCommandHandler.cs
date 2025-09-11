using MediatR;
using MusicManagementDemo.Application.Responses;
using MusicManagementDemo.Abstractions;
using RustSharp;

namespace MusicManagementDemo.Application.UseCase.Management.CancelJob;

public class CancelJobCommandHandler(IJobManager jobManager)
    : IRequestHandler<CancelJobCommand, IServiceResult>
{
    public Task<IServiceResult> Handle(
        CancelJobCommand request,
        CancellationToken cancellationToken
    )
    {
        var result = jobManager.CancelJob(request.JobId);
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
