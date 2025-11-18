using Mediator;
using MusicManagementDemo.Application.Responses;

namespace MusicManagementDemo.Application.UseCase.Identity.Register;

public sealed record RegisterCommand(string Email, string UserName, string Password)
    : IRequest<ApiResult<RegisterCommandResponse>>;
