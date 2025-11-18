using Mediator;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MusicManagementDemo.Abstractions;
using MusicManagementDemo.Application.Responses;
using MusicManagementDemo.Domain.Entity.Identity;
using static MusicManagementDemo.Application.Responses.ApiResult<MusicManagementDemo.Application.UseCase.Identity.Login.LoginCommandResponse>;

namespace MusicManagementDemo.Application.UseCase.Identity.Login;

internal sealed class LoginCommandHandler(
    UserManager<ApplicationUser> userMgr,
    SignInManager<ApplicationUser> signInMgr,
    IJwtManager jwtManager,
    IConfiguration config,
    ILogger<LoginCommandHandler> logger
) : IRequestHandler<LoginCommand, ApiResult<LoginCommandResponse>>
{
    public async ValueTask<ApiResult<LoginCommandResponse>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken
    )
    {
        var user = await userMgr.FindByEmailAsync(request.Email);
        if (user == null)
        {
            logger.LogError("User {userId} not found.", request.Email);
            return Err(5004, "邮箱或密码错误");
        }

        var signInRes = await signInMgr.CheckPasswordSignInAsync(user, request.Password, false);
        if (!signInRes.Succeeded)
        {
            logger.LogError(
                "User {userId} Sing in failed, reason: {@signInRes}",
                request.Email,
                signInRes
            );
            return Err(5004, "邮箱或密码错误");
        }

        var roles = await userMgr.GetRolesAsync(user);

        var tokenStr = jwtManager.GenerateJwtToken(
            user.Id.ToString(),
            user.UserName!,
            roles,
            user.ConcurrencyStamp ?? string.Empty,
            config
        );
        logger.LogInformation("User {userId} logged in.", user.Id);

        return Ok(new LoginCommandResponse(tokenStr));
    }
}
