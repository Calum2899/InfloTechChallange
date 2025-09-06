namespace UserManagement.Application.DTOs;

public record RegisterDto(string Email, string Password, string FirstName, string LastName);
public record LoginDto(string Email, string Password);
public record AuthResultDto(string Token, DateTime ExpiresUtc);
