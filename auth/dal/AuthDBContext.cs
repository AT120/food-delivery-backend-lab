using System.Runtime.CompilerServices;
using AuthDAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthDAL;

public class AuthDBContext : IdentityDbContext<User, Role, Guid>
{
    public DbSet<Cook> Cooks { get; set; }
    public DbSet<Courier> Couriers { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Manager> Managers { get; set; }
    public DbSet<IssuedToken> Tokens { get; set; }

    public AuthDBContext(DbContextOptions<AuthDBContext> options) : base(options) { }



    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        
        builder.Entity<Cook>().HasKey(c => c.BaseUserId);
        builder.Entity<Cook>()
            .HasOne(c => c.BaseUser)
            .WithOne()
            .HasForeignKey<Cook>(c => c.BaseUserId)
            .IsRequired();

        builder.Entity<Courier>().HasKey(c => c.BaseUserId);
        builder.Entity<Courier>()
            .HasOne(c => c.BaseUser)
            .WithOne()
            .HasForeignKey<Courier>(c => c.BaseUserId)
            .IsRequired();
        
        builder.Entity<Customer>().HasKey(c => c.BaseUserId);
        builder.Entity<Customer>()
            .HasOne(c => c.BaseUser)
            .WithOne()
            .HasForeignKey<Customer>(c => c.BaseUserId)
            .IsRequired();
        
        builder.Entity<Manager>().HasKey(c => c.BaseUserId);
        builder.Entity<Manager>()
            .HasOne(c => c.BaseUser)
            .WithOne()
            .HasForeignKey<Manager>(c => c.BaseUserId)
            .IsRequired();

        builder.Entity<IssuedToken>().Property(x => x.Id).UseIdentityColumn();
        builder.Entity<User>()
            .HasMany<IssuedToken>()
            .WithOne()
            .HasForeignKey(u => u.UserId)
            .IsRequired();

        builder.Entity<User>()
            .HasMany(u => u.Roles)
            .WithMany(r => r.Users)
            .UsingEntity<IdentityUserRole<Guid>>();
    }
}