using MediatR;
using RustSharp;

namespace MusicManagementDemo.Application.UseCase.Identity.Register;

internal sealed class RegisterCommandHandler
    : IRequestHandler<RegisterCommand, Result<Guid, string>>
{
    public async Task<Result<Guid, string>> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken
    )
    {
        return Result.Ok(Guid.NewGuid());
    }
}
