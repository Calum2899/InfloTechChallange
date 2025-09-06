using UserManagement.Domain.Entities;

namespace UserManagement.Application.Abstractions;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<List<User>> SearchAsync(bool? isActive, CancellationToken ct = default);
    Task<long> CreateAsync(User user, long modifiedBy = -1, CancellationToken ct = default);
    Task UpdateAsync(User user, long modifiedBy = -1, CancellationToken ct = default);
    Task DeleteAsync(long id, long modifiedBy = -1, CancellationToken ct = default);
    Task<List<Log>> GetLogsForUserAsync(long userId, int skip, int take, CancellationToken ct = default);
}
