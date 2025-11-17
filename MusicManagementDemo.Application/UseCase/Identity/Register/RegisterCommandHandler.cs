using Mediator;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MusicManagementDemo.Abstractions.IDbContext;
using MusicManagementDemo.Application.Responses;
using MusicManagementDemo.Domain.Entity.Identity;

namespace MusicManagementDemo.Application.UseCase.Identity.Register;

internal sealed class RegisterCommandHandler(
    UserManager<ApplicationUser> userMgr,
    IIdentityDbContext dbContext,
    ILogger<RegisterCommandHandler> logger
) : IRequestHandler<RegisterCommand, IServiceResult>
{
    public async ValueTask<IServiceResult> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken
    )
    {
        var isFirstUser = !await dbContext.Users.AnyAsync(cancellationToken: cancellationToken);
        var user = new ApplicationUser { UserName = request.UserName, Email = request.Email };
        var result = await userMgr.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            logger.LogError("Create user failed, reason: {@result}", result);
            return ApiResult<>.Err(503, [.. result.Errors.Select(e => e.Description)]);
        }
        logger.LogInformation("User {userId} registered.", user.Id);

        if (isFirstUser)
        {
            var adminRole = await dbContext.Roles.SingleOrDefaultAsync(
                e => e.NormalizedName == "ADMIN",
                cancellationToken: cancellationToken
            );
            if (adminRole is null)
            {
                logger.LogError("Role Admin is not exist.");
                return ApiResult<>.Err(503, ["内部错误"]);
            }
            await userMgr.AddToRoleAsync(user, adminRole.NormalizedName!);
            logger.LogInformation("User {userId} is admin.", user.Id);
        }
        return ApiResult<>.Ok(new RegisterCommandResponse(user.Id));
    }
}
