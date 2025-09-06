using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UserManagement.Domain.Entities;
using UserManagement.Infrastructure.Identity;

namespace UserManagement.Infrastructure.Persistence;

public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, long>
{
    public DbSet<User> DomainUsers => Set<User>();
    public DbSet<Log> Logs => Set<Log>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);

        b.Entity<User>(cfg =>
        {
            cfg.ToTable("Users");
            cfg.HasKey(x => x.Id);
            cfg.Property(x => x.Id)
                  .ValueGeneratedOnAdd()
                  .HasAnnotation("Sqlite:Autoincrement", true); cfg.Property(x => x.Forename).IsRequired().HasMaxLength(64);
            cfg.Property(x => x.Surname).IsRequired().HasMaxLength(100);
            cfg.Property(x => x.Email).IsRequired().HasMaxLength(256);
            cfg.HasIndex(x => x.Email).IsUnique();
            cfg.Property(x => x.DateOfBirth).IsRequired();
            cfg.Property(x => x.IsActive).IsRequired();
        });

        b.Entity<Log>(cfg =>
        {
            cfg.ToTable("Logs");
            cfg.HasKey(l => l.Id);
            cfg.Property(l => l.Id)
                  .ValueGeneratedOnAdd()
                  .HasAnnotation("Sqlite:Autoincrement", true); cfg.Property(l => l.Action).IsRequired().HasMaxLength(64);
            cfg.Property(l => l.Description).IsRequired();
            cfg.Property(l => l.Timestamp).IsRequired();

            cfg.HasOne(l => l.User)
               .WithMany()
               .HasForeignKey(l => l.UserId)
               .OnDelete(DeleteBehavior.Restrict);

            cfg.HasOne(l => l.UserThatModified)
               .WithMany()
               .HasForeignKey(l => l.ModifiedBy)
               .OnDelete(DeleteBehavior.Restrict);

            cfg.HasIndex(l => l.UserId);
            cfg.HasIndex(l => l.ModifiedBy);
        });
    }
}
