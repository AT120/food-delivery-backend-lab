using System.Collections.Immutable;
using AuthDAL;
using AuthDAL.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProjCommon;
using ProjCommon.Configurators;
using ProjCommon.Enums;

namespace AuthBL;
public static class AuthConfigurator
{


    public static void AddUserIdentityStorage(this WebApplicationBuilder builder, bool cookieEnabled = false)
    {
        builder.AddDB<AuthDBContext>("AuthConnection");

        if (cookieEnabled)
        {
            builder.Services.AddIdentity<User, Role>()
                .AddSignInManager<SignInManager<User>>()
                .AddEntityFrameworkStores<AuthDBContext>();
        }
        else
        {
            builder.Services.AddIdentityCore<User>()
                .AddRoles<Role>()
                .AddRoleManager<RoleManager<Role>>()
                .AddEntityFrameworkStores<AuthDBContext>();
        }

        builder.Services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredLength = 5;

            options.User.RequireUniqueEmail = true;
            options.ClaimsIdentity.UserIdClaimType = ClaimType.UserId;
        });
    }

    public static void MigrateAuthDB(this WebApplication app)
        => app.MigrateDBWhenNecessary<AuthDBContext>();

    public static async Task SeedRoles(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
            var existingRoles = await roleManager.Roles
                .Select(s => s.RoleType)
                .ToListAsync();

            var definedRoles = Enum.GetValues<RoleType>();
            // var rolesToDelete = existingRoles.Except(definedRoles);
            var rolesToCreate = definedRoles.Except(existingRoles);

            foreach (var role in rolesToCreate)
            {
                await roleManager.CreateAsync(new Role
                {
                    Name = role.ToString(),
                    RoleType = role,
                });
            }
        }
    }

    public static async Task InitAdmin(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        
            if (! userManager.Users.Any(u => u.Email == "root@root"))
            {
                var admin = new User {
                    Email = "admin@admin",
                    UserName = "admin@admin",
                    FullName = "Admin"
                };

                await userManager.CreateAsync(admin, "admin");
                await userManager.AddToRoleAsync(admin, RoleType.Admin.ToString());
            }
        }
    }
}