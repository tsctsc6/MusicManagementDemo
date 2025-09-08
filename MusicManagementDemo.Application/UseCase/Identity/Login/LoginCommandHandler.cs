using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MusicManagementDemo.Domain.Entity.Identity;
using MusicManagementDemo.Infrastructure.Responses;
using MusicManagementDemo.SharedKernel;

namespace MusicManagementDemo.Application.UseCase.Identity.Login;

internal sealed class LoginCommandHandler(
    UserManager<ApplicationUser> userMgr,
    SignInManager<ApplicationUser> signInMgr,
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

        // 生成 JWT
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.UniqueName, user.UserName),
        };
        // 加入角色声明
        var roles = await userMgr.GetRolesAsync(user);
        claims.AddRange(roles.Select(r => new Claim("role", r)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );
        var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);

        return ServiceResult<object>.Ok(new { token = tokenStr, expires = token.ValidTo });
    }
}
