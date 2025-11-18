using Mediator;
using MusicManagementDemo.Application.Responses;

namespace MusicManagementDemo.Application.UseCase.Management.ReadAllStorage;

public sealed record ReadAllStorageQuery(int? ReferenceId, int PageSize, bool Asc)
    : IRequest<ApiResult<ReadAllStorageQueryResponse>>;
