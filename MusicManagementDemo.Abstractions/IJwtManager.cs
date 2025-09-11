using Microsoft.Extensions.Configuration;

namespace MusicManagementDemo.Abstractions;

public interface IJwtManager
{
    public string GenerateJwtToken(
        string userId,
        string userName,
        IList<string> roles,
        string concurrencyStamp,
        IConfiguration config
    );
}
