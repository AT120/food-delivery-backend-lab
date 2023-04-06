using AuthDAL.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuthDAL;
public static class AuthConfigurator
{
    public static void AddAuth(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<AuthDBContext>(options =>
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
        });

        builder.Services.AddIdentity<User, Role>()
            .AddEntityFrameworkStores<AuthDBContext>();
    }

    public static void MigrateAuthDBWhenNecessary(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            if (bool.TryParse(config["RuntimeMigrations"], out bool migrate) && migrate)
            {
                var dbcontext = scope.ServiceProvider.GetRequiredService<AuthDBContext>();
                dbcontext.Database.Migrate();
            }
        }
    }
}