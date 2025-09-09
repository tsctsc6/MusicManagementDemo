using Microsoft.Extensions.Configuration;

namespace MusicManagementDemo.SharedKernel;

public interface IJwtManager
{
    public string GenerateJwtToken(
        string userId,
        string userName,
        IList<string> roles,
        IConfiguration config
    );
}
