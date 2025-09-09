using MediatR;
using MusicManagementDemo.Application.Responses;
using MusicManagementDemo.Domain.Entity.Management;
using MusicManagementDemo.Infrastructure.Database;
using MusicManagementDemo.SharedKernel;
using RustSharp;

namespace MusicManagementDemo.Application.UseCase.Management.CreateJob;

internal sealed class CreateJobCommandHandler(
    ManagementAppDbContext dbContext,
    IJobManager jobManager
) : IRequestHandler<CreateJobCommand, IServiceResult>
{
    public async Task<IServiceResult> Handle(
        CreateJobCommand request,
        CancellationToken cancellationToken
    )
    {
        var jobToAdd = new Job
        {
            Type = request.Type,
            Description = request.Description,
            Status = JobStatus.WaitingStart,
        };
        await dbContext.Job.AddAsync(jobToAdd, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        var result = jobManager.CreateJob(jobToAdd.Id, request.Type);
        return result switch
        {
            ErrResult<long, string> errResult => ServiceResult.Err(503, [errResult.Value]),
            OkResult<long, string> okResult => ServiceResult<object>.Ok(new { JobId = okResult.Value }),
            _ => ServiceResult.Err(503, ["内部错误"])
        };
    }
}
