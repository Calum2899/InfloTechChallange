namespace UserManagement.Application.DTOs;

public record UserDto(long Id, string Forename, string Surname, string Email, bool IsActive, DateOnly DateOfBirth);
public record CreateUserDto(string Forename, string Surname, string Email, bool IsActive, DateOnly DateOfBirth);
public record UpdateUserDto(string Forename, string Surname, string Email, bool IsActive, DateOnly DateOfBirth);
