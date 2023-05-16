using System.Reflection.Metadata;
using BackendDAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace BackendDAL;

public class BackendDBContext : DbContext
{
    public DbSet<Cook> Cooks { get; set; }
    public DbSet<Courier> Couriers { get; set; }
    // public DbSet<Customer> Customers { get; set; }
    public DbSet<Dish> Dishes { get; set; }
    public DbSet<DishInCart> DishesInCart { get; set; }
    public DbSet<Manager> Managers { get; set; }
    public DbSet<Menu> Menus { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderedDish> OrderedDishes { get; set; }
    public DbSet<RatedDish> RatedDishes { get; set; }
    public DbSet<Restaurant> Restaurants { get; set; }
    public object Task { get; set; }

    public BackendDBContext(DbContextOptions<BackendDBContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<DishInCart>().HasKey(d => new { d.CustomerId, d.DishId });
        builder.Entity<OrderedDish>().HasKey(d => new { d.OrderId, d.DishId });
        builder.Entity<RatedDish>().HasKey(d => new { d.CustomerId, d.DishId });
        builder.Entity<Order>().Property(o => o.Id).UseIdentityColumn();
        builder.Entity<Menu>().Property(m => m.Id).UseIdentityColumn();
        // builder.Entity<Customer>()
        //     .HasMany<Dish>(c => c.RatedDishes)
        //     .WithMany();

        builder.Entity<Restaurant>().HasIndex(r => r.Name).IsUnique();
        
        builder.Entity<Restaurant>()
            .HasMany(r => r.Cooks)
            .WithOne(c => c.Restaurant)
            .HasForeignKey(c => c.RestaurantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Restaurant>()
            .HasMany(r => r.Managers)
            .WithOne(c => c.Restaurant)
            .HasForeignKey(c => c.RestaurantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Cook>()
            .HasMany(c => c.Orders)
            .WithOne(o => o.Cook)
            .HasForeignKey(o => o.CookId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<Courier>()
            .HasMany(c => c.Orders)
            .WithOne(o => o.Courier)
            .HasForeignKey(o => o.CourierId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}