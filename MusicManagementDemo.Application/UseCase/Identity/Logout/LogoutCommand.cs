using Mediator;
using MusicManagementDemo.Application.Responses;

namespace MusicManagementDemo.Application.UseCase.Identity.Logout;

public sealed record LogoutCommand(Guid UserId) : IRequest<ApiResult<LogoutCommandResponse>>;
