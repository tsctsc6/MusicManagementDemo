using MediatR;
using MusicManagementDemo.Abstractions;

namespace MusicManagementDemo.Application.UseCase.Identity.Logout;

public sealed record LogoutCommand(string UserId) : IRequest<IServiceResult>;
