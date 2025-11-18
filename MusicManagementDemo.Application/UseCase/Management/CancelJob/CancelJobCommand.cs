using Mediator;
using MusicManagementDemo.Application.Responses;

namespace MusicManagementDemo.Application.UseCase.Management.CancelJob;

public sealed record CancelJobCommand(long JobId) : IRequest<ApiResult<CancelJobCommandResponse>>;
