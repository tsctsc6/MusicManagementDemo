using Mediator;

namespace MusicManagementDemo.Application.UseCase.Identity.Logout;

public sealed record LogoutCommand(Guid UserId) : IRequest<IServiceResult>;
