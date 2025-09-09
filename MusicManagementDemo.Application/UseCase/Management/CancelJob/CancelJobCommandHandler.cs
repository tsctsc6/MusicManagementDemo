using MediatR;
using MusicManagementDemo.Application.Responses;
using MusicManagementDemo.SharedKernel;

namespace MusicManagementDemo.Application.UseCase.Management.CancelJob;

public class CancelJobCommandHandler(IJobManager jobManager)
    : IRequestHandler<CancelJobCommand, IServiceResult>
{
    public Task<IServiceResult> Handle(
        CancelJobCommand request,
        CancellationToken cancellationToken
    )
    {
        jobManager.CancelJob(request.JobId);
        return Task.FromResult<IServiceResult>(ServiceResult.Ok());
    }
}
