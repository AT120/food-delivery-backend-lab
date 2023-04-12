using BackendDAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace BackendDAL;

public class BackendDBContext : DbContext
{
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Dish> Dishes { get; set; }
    public DbSet<DishInCart> DishesInCart { get; set; }
    public DbSet<Menu> Menus { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Restaurant> Restaurants { get; set; }
    
    public BackendDBContext(DbContextOptions<BackendDBContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

    }
}