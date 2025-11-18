using Mediator;
using Microsoft.Extensions.Logging;
using MusicManagementDemo.Abstractions;
using MusicManagementDemo.Abstractions.IDbContext;
using MusicManagementDemo.Application.Responses;
using MusicManagementDemo.Domain.Entity.Management;
using RustSharp;
using static MusicManagementDemo.Application.Responses.ApiResult<MusicManagementDemo.Application.UseCase.Management.CreateJob.CreateJobCommandResponse>;

namespace MusicManagementDemo.Application.UseCase.Management.CreateJob;

internal sealed class CreateJobCommandHandler(
    IManagementAppDbContext dbContext,
    IJobManager jobManager,
    ILogger<CreateJobCommandHandler> logger
) : IRequestHandler<CreateJobCommand, ApiResult<CreateJobCommandResponse>>
{
    public async ValueTask<ApiResult<CreateJobCommandResponse>> Handle(
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
                return Err(503, errResult.Value);
            case OkResult<long, string> okResult:
                logger.LogInformation("{err}", okResult.Value);
                return Ok(new CreateJobCommandResponse(okResult.Value));
            default:
                logger.LogInformation("Unknown type {@result}", result);
                return Err(503, "内部错误");
        }
    }
}
