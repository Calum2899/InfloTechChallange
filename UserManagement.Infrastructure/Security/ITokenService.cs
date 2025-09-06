using UserManagement.Infrastructure.Identity;

namespace UserManagement.Infrastructure.Security;

public interface ITokenService
{
    (string token, DateTime expiresUtc) CreateToken(ApplicationUser user, IEnumerable<string> roles);
}
