using Mediator;

namespace MusicManagementDemo.Application.UseCase.Identity.Login;

public sealed record LoginCommand(string Email, string Password) : IRequest<IServiceResult>;
