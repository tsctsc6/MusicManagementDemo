using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using RustSharp;

namespace MusicManagementDemo.WebApi.Utils;

public static class ClaimsPrincipalExtensions
{
    public static Option<Guid> GetUserId(this ClaimsPrincipal principal)
    {
        if (
            !Guid.TryParse(
                principal.Claims.SingleOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value,
                out var userId
            )
        )
        {
            return Option.None();
        }
        return Option.Some(userId);
    }
}
