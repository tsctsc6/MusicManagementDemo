using System.Text.Json.Nodes;
using Mediator;
using MusicManagementDemo.Domain.Entity.Management;

namespace MusicManagementDemo.Application.UseCase.Management.CreateJob;

public sealed record CreateJobCommand(JobType Type, string Description, JsonNode JobArgs)
    : IRequest<IServiceResult>;
