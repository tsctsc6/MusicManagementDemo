using MediatR;
using Microsoft.EntityFrameworkCore;
using MusicManagementDemo.Application.Responses;
using MusicManagementDemo.Infrastructure.Database;
using MusicManagementDemo.SharedKernel;

namespace MusicManagementDemo.Application.UseCase.Management.ReadJob;

public class ReadJobQueryHandler(ManagementAppDbContext dbContext)
    : IRequestHandler<ReadJobQuery, IServiceResult>
{
    public async Task<IServiceResult> Handle(
        ReadJobQuery request,
        CancellationToken cancellationToken
    )
    {
        var jobToRead = await dbContext
            .Job.AsNoTracking()
            .SingleOrDefaultAsync(j => j.Id == request.Id, cancellationToken: cancellationToken);
        if (jobToRead is null)
        {
            return ServiceResult.Err(406, ["Job not found"]);
        }
        return ServiceResult<object>.Ok(
            new
            {
                jobToRead.Id,
                Type = jobToRead.Type.ToString(),
                jobToRead.JobArgs,
                jobToRead.Status,
                jobToRead.Description,
                jobToRead.ErrorMesage,
                jobToRead.Success,
                jobToRead.CreatedAt,
                jobToRead.CompletedAt,
            }
        );
    }
}
