using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MusicManagementDemo.Abstractions;
using MusicManagementDemo.Application.Responses;
using MusicManagementDemo.Domain.Entity.Identity;

namespace MusicManagementDemo.Application.UseCase.Identity.Logout;

internal sealed class LogoutCommandHandler(
    IdentityDbContext<ApplicationUser> dbContext,
    UserManager<ApplicationUser> userManager,
    ILogger<LogoutCommandHandler> logger
) : IRequestHandler<LogoutCommand, IServiceResult>
{
    public async Task<IServiceResult> Handle(
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
            return ServiceResult.Err(404, ["用户 Id 不存在"]);
        }

        // 这一行对于 jwt 是没有用的。
        // await signInMgr.SignOutAsync();
        user.ConcurrencyStamp = await userManager.GenerateConcurrencyStampAsync(user);
        var submitCount = await dbContext.SaveChangesAsync(cancellationToken);
        if (submitCount == 0)
        {
            logger.LogError("ConcurrencyStamp of User {userId} not changed.", request.UserId);
            return ServiceResult.Err(404, ["登出失败"]);
        }

        logger.LogInformation("User {userId} logged out.", user.Id);
        return ServiceResult.Ok();
    }
}
