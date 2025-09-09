using System.Text.Json.Nodes;
using MediatR;
using MusicManagementDemo.SharedKernel;

namespace MusicManagementDemo.Application.UseCase.Management.CreateJob;

public sealed record CreateJobCommand(JobType Type, string Description, JsonNode JobArgs) : IRequest<IServiceResult>;
