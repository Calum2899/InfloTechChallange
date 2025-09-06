namespace UserManagement.Application.DTOs;

public record LogDto(
    long Id,
    long UserId,
    string Action,
    string Description,
    long ModifiedBy,
    DateTime Timestamp
);
