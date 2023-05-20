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
    // public async static Task UpdateRolesAndClaims(this WebApplication app)
    // {
    //     using (var scope = app.Services.CreateScope())
    //     {
    //         var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
    //         var existingRoles = roleManager.Roles.ToList();
    //         foreach (var rolesClaims in RolesAndClaims.Map)
    //         {
    //             // Create missing roles
    //             var roleType = rolesClaims.Key;
    //             var claims = rolesClaims.Value;
    //             Role? role = existingRoles.Find(x => x.RoleType == roleType);
    //             if (role is null)
    //             {
    //                 role = new Role
    //                 {
    //                     RoleType = roleType,
    //                     Name = Enum.GetName<RoleType>(roleType)
    //                 };
    //                 var res = await roleManager.CreateAsync(role);
    //                 if (!res.Succeeded)
    //                     throw new InvalidOperationException("Can't create role");
    //             }

    //             // Create missing claims for each role 
    //             var existingClaims = await roleManager.GetClaimsAsync(role);
    //             foreach (var claimType in claims)
    //             {
    //                 var claim = existingClaims.FirstOrDefault(x => x.Type == claimType.ToString());
    //                 if (claim is null)
    //                 {
    //                     claim = new Claim(claimType.ToString(), "");
    //                     var res = await roleManager.AddClaimAsync(role, claim);
    //                     if (!res.Succeeded)
    //                         throw new InvalidOperationException("Can't create role");
    //                 }
    //                 else
    //                 {
    //                     existingClaims.Remove(claim);
    //                 }
    //             }

    //             // Remove redundant claims
    //             foreach (var redClaim in existingClaims)
    //             {
    //                 var res = await roleManager.RemoveClaimAsync(role, redClaim);
    //                 if (!res.Succeeded)
    //                     throw new InvalidOperationException("Can't create role");
    //             }

    //         }
        // }
    // }


}