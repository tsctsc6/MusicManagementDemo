using Mediator;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MusicManagementDemo.Abstractions.IDbContext;
using MusicManagementDemo.Application.Responses;
using MusicManagementDemo.Domain.Entity.Identity;
using static MusicManagementDemo.Application.Responses.ApiResult<MusicManagementDemo.Application.UseCase.Identity.Logout.LogoutCommandResponse>;

namespace MusicManagementDemo.Application.UseCase.Identity.Logout;

internal sealed class LogoutCommandHandler(
    IIdentityDbContext dbContext,
    UserManager<ApplicationUser> userManager,
    ILogger<LogoutCommandHandler> logger
) : IRequestHandler<LogoutCommand, ApiResult<LogoutCommandResponse>>
{
    public async ValueTask<ApiResult<LogoutCommandResponse>> Handle(
        LogoutCommand request,
        CancellationToken cancellationToken
    )
    {
        var user = await dbContext.Users.SingleOrDefaultAsync(
            e => e.Id == request.UserId,
            cancellationToken: cancellationToken
        );
        if (user is null)
        {
            logger.LogError("User {userId} not found.", request.UserId);
            return Err(404, "用户 Id 不存在");
        }

        // 这一行对于 jwt 是没有用的。
        // await signInMgr.SignOutAsync();
        user.ConcurrencyStamp = await userManager.GenerateConcurrencyStampAsync(user);
        var submitCount = await dbContext.SaveChangesAsync(cancellationToken);
        if (submitCount == 0)
        {
            logger.LogError("ConcurrencyStamp of User {userId} not changed.", request.UserId);
            return Err(404, "登出失败");
        }

        logger.LogInformation("User {userId} logged out.", user.Id);
        return Ok(new LogoutCommandResponse());
    }
}
