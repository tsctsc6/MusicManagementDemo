using MediatR;
using MusicManagementDemo.Abstractions;
using MusicManagementDemo.Abstractions.IDbContext;
using MusicManagementDemo.Application.Responses;
using MusicManagementDemo.Domain.Entity.Management;
using RustSharp;

namespace MusicManagementDemo.Application.UseCase.Management.CreateJob;

internal sealed class CreateJobCommandHandler(
    IManagementAppDbContext dbContext,
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
            JobArgs = request.JobArgs,
            Description = request.Description,
            Status = JobStatus.WaitingStart,
        };
        await dbContext.Job.AddAsync(jobToAdd, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        var result = jobManager.CreateJob(jobToAdd.Id, request.Type);
        return result switch
        {
            ErrResult<long, string> errResult => ServiceResult.Err(503, [errResult.Value]),
            OkResult<long, string> okResult => ServiceResult.Ok(new { JobId = okResult.Value }),
            _ => ServiceResult.Err(503, ["内部错误"]),
        };
    }
}
