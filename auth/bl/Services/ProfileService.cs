using System.Security.Claims;
using System.Security.Cryptography;
using AuthCommon.DTO;
using AuthCommon.Interfaces;
using AuthDAL;
using AuthDAL.Models;
using Microsoft.AspNetCore.Identity;
using ProjCommon.Enums;
using ProjCommon.Exceptions;

namespace AuthBL.Services;

public class ProfileService : IProfileService
{
    private readonly UserManager<User> _userManager;
    private readonly AuthDBContext _dbcontext;
    private readonly RoleManager<Role> _roleManager;

    public ProfileService(
        UserManager<User> um,
        AuthDBContext dbc,
        RoleManager<Role> rm)
    {
        _userManager = um;
        _dbcontext = dbc;
        _roleManager = rm;
    }


    //TODO: аче
    // public static async Task CreateCustomer(User user, string address, )
    // {
    //     var res = await _userManager.AddClaimAsync(
    //         user,
    //         ClaimsHelper.CreateClaim(ClaimType.Address, address)
    //     );
    //     if (!res.Succeeded)
    //         throw new BackendException(
    //             500,
    //             "An error occured while creating the user",
    //             $"Failed to create claim {res.Errors}"
    //         );
    //     res = await _userManager.AddToRoleAsync(user, Enum.GetName(RoleType.Customer));
    //     if (!res.Succeeded)
    //         throw new BackendException(
    //             500,
    //             "An error occured while creating the user",
    //             $"Failed to add user to role {res.Errors}"
    //         );

    //     await _dbcontext.Customers.AddAsync(new Customer
    //     {
    //         Address = address,
    //         BaseUser = user,
    //         Id = user.Id
    //     });

    //     await _dbcontext.SaveChangesAsync();
    // }


    public async Task<UserProfile> GetUserProfile(ClaimsPrincipal userPrincipal)
    {
        var user = await _userManager.GetUserAsync(userPrincipal)
            ?? throw new BackendException(404, "Requested user does not exist.");
        var claims = await _userManager.GetClaimsAsync(user);
        var roles = await _userManager.GetRolesAsync(user);

        var addr = claims.FirstOrDefault(x => x.Type == ClaimType.Address.ToString())?.Value;
        return new UserProfile
        {
            BirthDate = user.BirthDate,
            Email = user.Email,
            FullName = user.FullName,
            Gender = user.Gender,
            Roles = roles,
            PhoneNumber = user.PhoneNumber,
            Address = addr,
        };
    }

    public async Task UpdateUserProfile(UserProfileEdit newProfile, ClaimsPrincipal userPrincipal)
    {
        var user = await _userManager.GetUserAsync(userPrincipal)
            ?? throw new BackendException(404, "Requested user does not exist.");

        user.BirthDate = newProfile.BirthDate ?? user.BirthDate;
        user.FullName = newProfile.Name ?? user.FullName;
        user.Gender = newProfile.Gender ?? user.Gender;
        user.PhoneNumber = newProfile.PhoneNumber ?? user.PhoneNumber;



    }
}