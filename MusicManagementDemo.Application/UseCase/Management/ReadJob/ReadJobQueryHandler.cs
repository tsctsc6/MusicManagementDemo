using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MusicManagementDemo.Abstractions.IDbContext;
using MusicManagementDemo.Application.Responses;
using static MusicManagementDemo.Application.Responses.ApiResult<MusicManagementDemo.Application.UseCase.Management.ReadJob.ReadJobQueryResponse>;

namespace MusicManagementDemo.Application.UseCase.Management.ReadJob;

internal sealed class ReadJobQueryHandler(
    IManagementAppDbContext dbContext,
    ILogger<ReadJobQueryHandler> logger
) : IRequestHandler<ReadJobQuery, ApiResult<ReadJobQueryResponse>>
{
    public async ValueTask<ApiResult<ReadJobQueryResponse>> Handle(
        ReadJobQuery request,
        CancellationToken cancellationToken
    )
    {
        var jobToRead = await dbContext
            .Jobs.AsNoTracking()
            .SingleOrDefaultAsync(j => j.Id == request.Id, cancellationToken: cancellationToken);
        if (jobToRead is null)
        {
            logger.LogError("Job {RequestId} not found", request.Id);
            return Err(406, "Job not found");
        }
        return Ok(
            new ReadJobQueryResponse(
                jobToRead.Id,
                jobToRead.Type,
                jobToRead.JobArgs.ToJsonString(),
                jobToRead.Status,
                jobToRead.Description,
                jobToRead.ErrorMesage,
                jobToRead.Success,
                jobToRead.CreatedAt,
                jobToRead.CompletedAt
            )
        );
    }
}
