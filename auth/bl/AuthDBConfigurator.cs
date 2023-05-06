using AuthDAL;
using AuthDAL.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using ProjCommon;

namespace AuthBL;
public static class AuthConfigurator
{
    public static void AddUserIdentityStorage(this WebApplicationBuilder builder)
    {
        builder.AddDB<AuthDBContext>("AuthConnection");
        builder.Services.AddIdentityCore<User>()
            .AddRoles<Role>()
            .AddRoleManager<RoleManager<Role>>()
            .AddEntityFrameworkStores<AuthDBContext>();     
    }

    public static void MigrateAuthDB(this WebApplication app)
        => app.MigrateDBWhenNecessary<AuthDBContext>();
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