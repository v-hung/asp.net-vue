using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using BookManagement.Server.Core.Models;

namespace BookManagement.Server.Data;

public class ApplicationDbContext : IdentityDbContext<User, Role, Guid>
{
    public DbSet<MenuItem> MenuItems { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    public ApplicationDbContext(DbContextOptions options) : base(options) { }

    public override int SaveChanges()
    {
        var entries = ChangeTracker.Entries().Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            List<string> properties = entry.Metadata.GetProperties().Select(v => v.Name).ToList();

            if (entry.State == EntityState.Added && properties.Contains("CreatedAt"))
            {
                entry.Property("CreatedAt").CurrentValue = DateTime.UtcNow;
            }
            else if (entry.State == EntityState.Modified && properties.Contains("UpdatedAt"))
            {
                entry.Property("UpdatedAt").CurrentValue = DateTime.UtcNow;
            }

        }

        return base.SaveChanges();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<User>(b =>
        {
            b.HasMany(e => e.Roles)
            .WithMany(e => e.Users)
            .UsingEntity<IdentityUserRole<Guid>>();
        });

    }

}