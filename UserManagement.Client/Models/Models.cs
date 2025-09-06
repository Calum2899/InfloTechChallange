namespace UserManagement.Client.Models;

public record UserDto(long Id, string Forename, string Surname, string Email, bool IsActive, DateOnly DateOfBirth);
public record CreateUserDto(string Forename, string Surname, string Email, bool IsActive, DateOnly DateOfBirth);
public record UpdateUserDto(string Forename, string Surname, string Email, bool IsActive, DateOnly DateOfBirth);

public record LoginDto(string Email, string Password);
public record AuthResultDto(string Token, DateTime ExpiresUtc);
