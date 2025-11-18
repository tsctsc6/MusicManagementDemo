using Mediator;
using MusicManagementDemo.Application.Responses;

namespace MusicManagementDemo.Application.UseCase.Management.ReadStorage;

public sealed record ReadStorageQuery(int Id) : IRequest<ApiResult<ReadStorageQueryResponse>>;
