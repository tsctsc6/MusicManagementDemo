using Mediator;
using MusicManagementDemo.Application.Responses;

namespace MusicManagementDemo.Application.UseCase.Management.ReadJob;

public sealed record ReadJobQuery(long Id) : IRequest<ApiResult<ReadJobQueryResponse>>;
