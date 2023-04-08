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

    public AuthDBContext(DbContextOptions<AuthDBContext> options) : base(options) { }



    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Cook>()
            .HasOne(c => c.BaseUser)
            .WithOne()
            .HasForeignKey<Cook>(c => c.Id)
            .IsRequired();

        builder.Entity<Courier>()
            .HasOne(c => c.BaseUser)
            .WithOne()
            .HasForeignKey<Courier>(c => c.Id)
            .IsRequired();
        
        builder.Entity<Customer>()
            .HasOne(c => c.BaseUser)
            .WithOne()
            .HasForeignKey<Customer>(c => c.Id)
            .IsRequired();
        
        builder.Entity<Manager>()
            .HasOne(c => c.BaseUser)
            .WithOne()
            .HasForeignKey<Manager>(c => c.Id)
            .IsRequired();
    }
}