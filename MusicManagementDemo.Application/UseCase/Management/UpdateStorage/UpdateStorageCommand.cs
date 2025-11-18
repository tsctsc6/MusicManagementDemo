using Mediator;
using MusicManagementDemo.Application.Responses;

namespace MusicManagementDemo.Application.UseCase.Management.UpdateStorage;

public sealed record UpdateStorageCommand(int Id, string Name, string Path)
    : IRequest<ApiResult<UpdateStorageCommandResponse>>;
