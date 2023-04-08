using System.Runtime.InteropServices;
using System.Security.Claims;
using AuthDAL.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjCommon;
using ProjCommon.Enums;

namespace AuthDAL;
public static class AuthConfigurator
{
    public static void AddIdentityStorage(this WebApplicationBuilder builder)
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

    public async static Task UpdateRolesAndClaims(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
            var existingRoles = roleManager.Roles.ToList();
            foreach (var rolesClaims in RolesAndClaims.Map)
            {

                // Create missing roles
                var roleType = rolesClaims.Key;
                var claims = rolesClaims.Value;
                Role? role = existingRoles.Find(x => x.RoleType == roleType);
                if (role is null)
                {
                    role = new Role
                    {
                        RoleType = roleType,
                        Name = Enum.GetName<RoleType>(roleType)
                    };
                    var res = await roleManager.CreateAsync(role);
                    if (!res.Succeeded)
                        throw new InvalidOperationException("Can't create role");
                }

                // Create missing claims for each role 
                var existingClaims = await roleManager.GetClaimsAsync(role);
                foreach (var claimType in claims)
                {
                    var claim = existingClaims.FirstOrDefault(x => x.Type == claimType.ToString());
                    if (claim is null)
                    {
                        claim = new Claim(claimType.ToString(), "");
                        var res = await roleManager.AddClaimAsync(role, claim);
                        if (!res.Succeeded)
                            throw new InvalidOperationException("Can't create role");
                    }
                    else
                    {
                        existingClaims.Remove(claim);
                    }
                }

                // Remove redundant claims
                foreach (var redClaim in existingClaims)
                {
                    var res = await roleManager.RemoveClaimAsync(role, redClaim);
                    if (!res.Succeeded)
                        throw new InvalidOperationException("Can't create role");
                }

            }


        }
    }


}