using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ProjCommon;

public static class BackendDBConfigurator
{
    public static void AddDB<T>(this WebApplicationBuilder builder) where T : DbContext
    {
        builder.Services.AddDbContext<T>(options =>
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
        });
    }

    public static void MigrateDBWhenNecessary<T>(this WebApplication app) where T : DbContext
    {
        using (var scope = app.Services.CreateScope())
        {
            var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            if (bool.TryParse(config["RuntimeMigrations"], out bool migrate) && migrate)
            {
                var dbcontext = scope.ServiceProvider.GetRequiredService<T>();
                dbcontext.Database.Migrate();
            }
        }
    }
}