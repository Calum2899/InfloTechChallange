using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using UserManagement.Models;

namespace UserManagement.Data
{
    public class DataContext : DbContext, IDataContext
    {
        public DataContext() => Database.EnsureCreated();

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseInMemoryDatabase("UserManagement.Data.DataContext");

        protected override void OnModelCreating(ModelBuilder model)
        {

            // Seed Users
            var users = new[]
            {
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

            model.Entity<User>().HasData(users);
            //seed logs
            var logs = users.Select(u => new Log
            {
                Id = u.Id, // use same ID as User
                UserId = u.Id,
                Action = "Create",
                Description = $"User {u.Forename} {u.Surname} created by system.",
                ModifiedBy = 0, // system
                Timestamp = DateTime.UtcNow
            }).ToArray();

            model.Entity<Log>().HasData(logs);

            // Configure Logs
            model.Entity<Log>(entity =>
            {
                entity.HasKey(l => l.Id);
                entity.Property(l => l.Action).IsRequired();
                entity.Property(l => l.Description).IsRequired();

                entity.HasOne<User>()
                      .WithMany()
                      .HasForeignKey(l => l.UserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<User>()
                      .WithMany()
                      .HasForeignKey(l => l.ModifiedBy)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }

        public DbSet<User>? Users { get; set; }
        public DbSet<Log>? Logs { get; set; }

        public IQueryable<TEntity> GetAll<TEntity>() where TEntity : class
            => base.Set<TEntity>();

        public void Create<TEntity>(TEntity entity, long modifiedBy = 0) where TEntity : class
        {
            base.Add(entity);
            SaveChanges();

            if (entity is User user)
                AddLog(user.Id, "Create", $"User {user.Forename} {user.Surname} created.", modifiedBy);
        }

        public void Update<TEntity>(TEntity entity, long modifiedBy = 0) where TEntity : class
        {
            base.Update(entity);
            SaveChanges();

            if (entity is User user)
                AddLog(user.Id, "Update", $"User {user.Forename} {user.Surname} updated.", modifiedBy);
        }

        public void Delete<TEntity>(TEntity entity, long modifiedBy = 0) where TEntity : class
        {
            base.Remove(entity);
            SaveChanges();

            if (entity is User user)
                AddLog(user.Id, "Delete", $"User {user.Forename} {user.Surname} deleted.", modifiedBy);
        }

        // Helper method to create a log
        private void AddLog(long userId, string action, string description, long modifiedBy)
        {
            // Treat 0 as system
            var actualModifiedBy = modifiedBy == 0 ? 0 : modifiedBy;

            var log = new Log
            {
                UserId = userId,
                Action = action,
                Description = description,
                ModifiedBy = actualModifiedBy,
                Timestamp = DateTime.UtcNow
            };

            base.Add(log);
            SaveChanges();
        }
    }
}
