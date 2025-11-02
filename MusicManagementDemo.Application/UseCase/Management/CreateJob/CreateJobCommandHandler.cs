using Mediator;
using Microsoft.Extensions.Logging;
using MusicManagementDemo.Abstractions;
using MusicManagementDemo.Abstractions.IDbContext;
using MusicManagementDemo.Application.Responses;
using MusicManagementDemo.Domain.Entity.Management;
using RustSharp;

namespace MusicManagementDemo.Application.UseCase.Management.CreateJob;

internal sealed class CreateJobCommandHandler(
    IManagementAppDbContext dbContext,
    IJobManager jobManager,
    ILogger<CreateJobCommandHandler> logger
) : IRequestHandler<CreateJobCommand, IServiceResult>
{
    public async ValueTask<IServiceResult> Handle(
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
        await dbContext.Jobs.AddAsync(jobToAdd, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        var result = await jobManager.CreateJobAsync(jobToAdd.Id, request.Type, cancellationToken);
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
