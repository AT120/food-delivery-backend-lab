using Microsoft.EntityFrameworkCore.Design;
using BackendDAL;
using Microsoft.EntityFrameworkCore;

class AuthDBContextFactory : IDesignTimeDbContextFactory<BackendDBContext>
{
    public BackendDBContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<BackendDBContext>();

        //TODO: а откуда?
        options.UseNpgsql("Server=127.0.0.1;Database=adv-backend-backend;User Id=postgres;Password=postgres;Port=5432"); 
        return new BackendDBContext(options.Options);
    }
} 