using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MusicManagementDemo.Abstractions;
using MusicManagementDemo.Application.Responses;
using MusicManagementDemo.Domain.Entity.Identity;

namespace MusicManagementDemo.Application.UseCase.Identity.Register;

internal sealed class RegisterCommandHandler(
    UserManager<ApplicationUser> userMgr,
    IdentityDbContext<ApplicationUser> dbContext
) : IRequestHandler<RegisterCommand, IServiceResult>
{
    public async Task<IServiceResult> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken
    )
    {
        var isFirstUser = !await dbContext.Users.AnyAsync(cancellationToken: cancellationToken);
        var user = new ApplicationUser { UserName = request.UserName, Email = request.Email };
        var result = await userMgr.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            return ServiceResult.Err(503, [.. result.Errors.Select(e => e.Description)]);
        }

        if (isFirstUser)
        {
            var adminRole = await dbContext.Roles.SingleOrDefaultAsync(
                e => e.NormalizedName == "ADMIN",
                cancellationToken: cancellationToken
            );
            if (adminRole is null)
            {
                return ServiceResult.Err(503, ["内部错误"]);
            }
            await userMgr.AddToRoleAsync(user, adminRole.NormalizedName!);
        }
        return ServiceResult.Ok(Guid.Parse(user.Id));
    }
}
