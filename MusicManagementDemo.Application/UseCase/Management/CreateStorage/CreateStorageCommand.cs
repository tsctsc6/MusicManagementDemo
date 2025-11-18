using Mediator;
using MusicManagementDemo.Application.Responses;

namespace MusicManagementDemo.Application.UseCase.Management.CreateStorage;

public sealed record CreateStorageCommand(string Name, string Path)
    : IRequest<ApiResult<CreateStorageCommandResponse>>;
