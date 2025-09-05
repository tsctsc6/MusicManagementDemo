using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using MusicManagementDemo.Domain.Identity;
using RustSharp;

namespace MusicManagementDemo.Application.UseCase.Identity.Register;

internal sealed class RegisterCommandHandler(UserManager<ApplicationUser> userMgr)
    : IRequestHandler<RegisterCommand, Result<Guid, string>>
{
    public async Task<Result<Guid, string>> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken
    )
    {
        var user = new ApplicationUser { UserName = request.UserName, Email = request.Email };
        var result = await userMgr.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            return Result.Err(string.Join(" ", result.Errors.Select(e => e.Description)));
        }
        return Result.Ok(Guid.Parse(user.Id));
    }
}
