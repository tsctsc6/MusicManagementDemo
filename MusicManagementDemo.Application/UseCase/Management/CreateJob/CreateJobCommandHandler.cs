using MediatR;
using MusicManagementDemo.Application.Responses;
using MusicManagementDemo.Domain.Entity.Management;
using MusicManagementDemo.Infrastructure.Database;
using MusicManagementDemo.SharedKernel;

namespace MusicManagementDemo.Application.UseCase.Management.CreateJob;

internal sealed class CreateJobCommandHandler(
    ManagementAppDbContext dbContext,
    IJobHandler jobHandler
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
        await dbContext.Job.AddAsync(
            jobToAdd,
            cancellationToken
        );
        await dbContext.SaveChangesAsync(cancellationToken);
        _ = jobHandler.Handle(jobToAdd.Id, request.Type);
        return ServiceResult.Ok();
    }
}
