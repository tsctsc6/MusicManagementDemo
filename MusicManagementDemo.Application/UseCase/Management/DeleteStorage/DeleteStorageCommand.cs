using Mediator;
using MusicManagementDemo.Application.Responses;

namespace MusicManagementDemo.Application.UseCase.Management.DeleteStorage;

public sealed record DeleteStorageCommand(int Id)
    : IRequest<ApiResult<DeleteStorageCommandResponse>>;
