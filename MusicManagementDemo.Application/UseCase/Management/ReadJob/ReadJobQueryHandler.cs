using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MusicManagementDemo.Abstractions;
using MusicManagementDemo.Abstractions.IDbContext;
using MusicManagementDemo.Application.Responses;

namespace MusicManagementDemo.Application.UseCase.Management.ReadJob;

internal sealed class ReadJobQueryHandler(
    IManagementAppDbContext dbContext,
    ILogger<ReadJobQueryHandler> logger
) : IRequestHandler<ReadJobQuery, IServiceResult>
{
    public async ValueTask<IServiceResult> Handle(
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
            return ServiceResult.Err(406, ["Job not found"]);
        }
        return ServiceResult.Ok(
            new
            {
                jobToRead.Id,
                Type = jobToRead.Type.ToString(),
                jobToRead.JobArgs,
                Status = jobToRead.Status.ToString(),
                jobToRead.Description,
                jobToRead.ErrorMesage,
                jobToRead.Success,
                jobToRead.CreatedAt,
                jobToRead.CompletedAt,
            }
        );
    }
}
