using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserManagement.Domain.Entities;
using UserManagement.Infrastructure.Identity;
using UserManagement.Infrastructure.Persistence;

namespace UserManagement.Api.Extensions;

public static class StartupSeed
{
    public static async Task ApplyMigrationsAndSeedAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var um = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var log = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("Seeder");

        await db.Database.MigrateAsync();

        // Seed Identity admin user (separate from domain users)
        if (!await um.Users.AnyAsync())
        {
            var admin = new ApplicationUser
            {
                UserName = "admin@example.com",
                Email = "admin@example.com",
                Forename = "Admin",
                Surname = "User",
                IsActive = true
            };
            var created = await um.CreateAsync(admin, "Passw0rd!");
            if (!created.Succeeded)
                throw new Exception(string.Join("; ", created.Errors.Select(e => e.Description)));
        }

        if (await db.Users.AnyAsync())
        {
            log.LogInformation("Domain users already present; skipping seed.");
            return;
        }

        var users = new[]
        {
                new User { Id = -1L, Forename = "System", Surname = "User", DateOfBirth = new DateOnly(2000, 01, 01), Email = "system@example.com", IsActive = false },
                new User { Id = 1, Forename = "Peter", Surname = "Loew", DateOfBirth = new DateOnly(2000, 01, 01), Email = "ploew@example.com", IsActive = true },
                new User { Id = 2, Forename = "Benjamin Franklin", Surname = "Gates", DateOfBirth = new DateOnly(2000, 01, 01), Email = "bfgates@example.com", IsActive = true },
                new User { Id = 3, Forename = "Castor", Surname = "Troy", DateOfBirth = new DateOnly(2000, 01, 01), Email = "ctroy@example.com", IsActive = false },
                new User { Id = 4, Forename = "Memphis", Surname = "Raines", DateOfBirth = new DateOnly(2000, 01, 01), Email = "mraines@example.com", IsActive = true },
                new User { Id = 5, Forename = "Stanley", Surname = "Goodspeed", DateOfBirth = new DateOnly(2000, 01, 01), Email = "sgodspeed@example.com", IsActive = true },
                new User { Id = 6, Forename = "H.I.", Surname = "McDunnough", DateOfBirth = new DateOnly(2000, 01, 01), Email = "himcdunnough@example.com", IsActive = true },
                new User { Id = 7, Forename = "Cameron", Surname = "Poe", DateOfBirth = new DateOnly(2000, 01, 01), Email = "cpoe@example.com", IsActive = false },
                new User { Id = 8, Forename = "Edward", Surname = "Malus", DateOfBirth = new DateOnly(2000, 01, 01), Email = "emalus@example.com", IsActive = false },
                new User { Id = 9, Forename = "Damon", Surname = "Macready", DateOfBirth = new DateOnly(2000, 01, 01), Email = "dmacready@example.com", IsActive = false },
                new User { Id = 10, Forename = "Johnny", Surname = "Blaze", DateOfBirth = new DateOnly(2000, 01, 01), Email = "jblaze@example.com", IsActive = true },
                new User { Id = 11, Forename = "Robin", Surname = "Feld", DateOfBirth = new DateOnly(2000, 01, 01), Email = "rfeld@example.com", IsActive = true },
        };

        db.Set<User>().AddRange(users);
        await db.SaveChangesAsync();

        var now = DateTime.UtcNow;
        var logs = users
            .Where(u => u.Id != -1) // skip system user
            .Select(u => new Log
            {
                Id = u.Id,
                UserId = u.Id,
                Action = "Create",
                Description = $"User {u.Forename} {u.Surname} created by system.",
                ModifiedBy = -1,
                Timestamp = now
            })
            .ToArray();

        db.Set<Log>().AddRange(logs);
        await db.SaveChangesAsync();

        log.LogInformation("Seeded {UserCount} users and {LogCount} logs.", users.Length, logs.Length);
    }
}
