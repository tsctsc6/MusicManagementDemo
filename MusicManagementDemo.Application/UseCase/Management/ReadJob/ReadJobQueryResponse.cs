using MusicManagementDemo.Domain.Entity.Management;

namespace MusicManagementDemo.Application.UseCase.Management.ReadJob;

public sealed record ReadJobQueryResponse(
    long Id,
    JobType Type,
    string JobArgs,
    JobStatus Status,
    string Description,
    string ErrorMessage,
    bool Success,
    DateTime CreatedAt,
    DateTime? CompletedAt
);
