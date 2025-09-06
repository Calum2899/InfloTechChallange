
using UserManagement.Infrastructure.Identity;

namespace UserManagement.Infrastructure.Security;
public interface IAuthService
{
    Task<ApplicationUser?> ValidateCredentials(string email, string password);
}

