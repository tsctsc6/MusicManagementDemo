using MediatR;
using MusicManagementDemo.Domain.Entity.Management;
using MusicManagementDemo.SharedKernel;

namespace MusicManagementDemo.Application.UseCase.Management.CreateJob;

public sealed record CreateJobCommand(JobType Type, string Description) : IRequest<IServiceResult>;
