using Mediator;
using MusicManagementDemo.Application.Responses;

namespace MusicManagementDemo.Application.UseCase.Identity.Login;

public sealed record LoginCommand(string Email, string Password)
    : IRequest<ApiResult<LoginCommandResponse>>;
