using MediatR;
using Microsoft.AspNetCore.Identity;
using MusicManagementDemo.Domain.Entity.Identity;
using MusicManagementDemo.Infrastructure.Responses;
using MusicManagementDemo.SharedKernel;

namespace MusicManagementDemo.Application.UseCase.Identity.Register;

internal sealed class RegisterCommandHandler(UserManager<ApplicationUser> userMgr)
    : IRequestHandler<RegisterCommand, IServiceResult>
{
    public async Task<IServiceResult> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken
    )
    {
        var user = new ApplicationUser { UserName = request.UserName, Email = request.Email };
        var result = await userMgr.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            return ServiceResult.Err(503, [.. result.Errors.Select(e => e.Description)]);
        }
        return ServiceResult<Guid>.Ok(Guid.Parse(user.Id));
    }
}
