using MediatR;
using RustSharp;

namespace MusicManagementDemo.Application.UseCase.Identity.Register;

public sealed record RegisterCommand(string Email, string UserName, string Password)
    : IRequest<Result<Guid, string>>;
