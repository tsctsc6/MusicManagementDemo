using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MusicManagementDemo.Abstractions;
using MusicManagementDemo.Application.Responses;
using MusicManagementDemo.Domain.Entity.Identity;

namespace MusicManagementDemo.Application.UseCase.Identity.Login;

internal sealed class LoginCommandHandler(
    UserManager<ApplicationUser> userMgr,
    SignInManager<ApplicationUser> signInMgr,
    IJwtManager jwtManager,
    IConfiguration config
) : IRequestHandler<LoginCommand, IServiceResult>
{
    public async Task<IServiceResult> Handle(
        LoginCommand request,
        CancellationToken cancellationToken
    )
    {
        var user = await userMgr.FindByEmailAsync(request.Email);
        if (user == null)
            return ServiceResult.Err(5004, ["邮箱或密码错误"]);

        var signInRes = await signInMgr.CheckPasswordSignInAsync(user, request.Password, false);
        if (!signInRes.Succeeded)
            return ServiceResult.Err(5004, ["邮箱或密码错误"]);

        var roles = await userMgr.GetRolesAsync(user);

        var tokenStr = jwtManager.GenerateJwtToken(
            user.Id,
            user.UserName!,
            roles,
            user.ConcurrencyStamp ?? string.Empty,
            config
        );

        return ServiceResult.Ok(new { token = tokenStr });
    }
}
