using Microsoft.AspNetCore.Identity;

namespace UserManagement.Infrastructure.Identity;

public class ApplicationUser : IdentityUser<long>
{
    public bool IsActive { get; set; } = true;
    public DateOnly DateOfBirth { get; set; } = default;
    public string? Forename { get; set; }
    public string? Surname { get; set; }
}
