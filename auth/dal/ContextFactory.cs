using Microsoft.EntityFrameworkCore.Design;
using AuthDAL;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;

class AuthDBContextFactory : IDesignTimeDbContextFactory<AuthDBContext>
{
    public AuthDBContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<AuthDBContext>();
        options.UseNpgsql("Server=127.0.0.1;Database=adv-backend-auth;User Id=postgres;Password=postgres;Port=5432"); 
        return new AuthDBContext(options.Options);
    }
} 