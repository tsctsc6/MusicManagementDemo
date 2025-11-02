using Mediator;
using MusicManagementDemo.Abstractions;

namespace MusicManagementDemo.Application.UseCase.Identity.Logout;

public sealed record LogoutCommand(Guid UserId) : IRequest<IServiceResult>;
