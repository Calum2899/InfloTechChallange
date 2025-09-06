using Microsoft.EntityFrameworkCore;
using UserManagement.Application.Abstractions;
using UserManagement.Domain.Entities;
using UserManagement.Infrastructure.Persistence;

namespace UserManagement.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;
    public UserRepository(AppDbContext db) => _db = db;

    public async Task<User?> GetByIdAsync(long id, CancellationToken ct = default) =>
        await _db.DomainUsers.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, ct);

    public async Task<List<User>> SearchAsync(bool? isActive, CancellationToken ct = default)
    {
        var q = _db.DomainUsers.AsNoTracking().AsQueryable();
        if (isActive is not null) q = q.Where(x => x.IsActive == isActive);
        return await q.OrderBy(x => x.Surname).ThenBy(x => x.Forename).ToListAsync(ct);
    }

    public async Task<long> CreateAsync(User user, long modifiedBy = -1, CancellationToken ct = default)
    {
        _db.DomainUsers.Add(user);
        await _db.SaveChangesAsync(ct);

        await AddLogAsync(user.Id, "Create", $"User {user.Forename} {user.Surname} created.", modifiedBy, ct);
        return user.Id;
    }

    public async Task UpdateAsync(User user, long modifiedBy = -1, CancellationToken ct = default)
    {
        _db.DomainUsers.Update(user);
        await _db.SaveChangesAsync(ct);

        await AddLogAsync(user.Id, "Update", $"User {user.Forename} {user.Surname} updated.", modifiedBy, ct);
    }

    public async Task DeleteAsync(long id, long modifiedBy = -1, CancellationToken ct = default)
    {
        var entity = await _db.DomainUsers.FirstOrDefaultAsync(u => u.Id == id, ct);
        if (entity is null) return;

        _db.DomainUsers.Remove(entity);
        await _db.SaveChangesAsync(ct);

        await AddLogAsync(id, "Delete", $"User {entity.Forename} {entity.Surname} deleted.", modifiedBy, ct);
    }

    public async Task<List<Log>> GetLogsForUserAsync(long userId, int skip, int take, CancellationToken ct = default) =>
        await _db.Logs.AsNoTracking()
            .Where(l => l.UserId == userId)
            .OrderByDescending(l => l.Timestamp)
            .Skip(skip).Take(take)
            .ToListAsync(ct);

    private async Task AddLogAsync(long userId, string action, string description, long modifiedBy, CancellationToken ct)
    {
        var log = new Log
        {
            UserId = userId,
            Action = action,
            Description = description,
            ModifiedBy = modifiedBy == -1 ? -1 : modifiedBy,
            Timestamp = DateTime.UtcNow
        };
        _db.Logs.Add(log);
        await _db.SaveChangesAsync(ct);
    }
}
