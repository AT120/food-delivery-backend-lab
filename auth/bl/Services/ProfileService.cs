using System.Security.Claims;
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

    public override bool Equals(object? obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public async Task<UserProfile> GetUserProfile(ClaimsPrincipal userPrincipal)
    {
        var user = await _userManager.GetUserAsync(userPrincipal)
            ?? throw new BackendException(404, "Requested user does not exist.");
        var roles = await _userManager.GetRolesAsync(user);


        var customer = await  _dbcontext.Customers
                .FindAsync(user.Id);
        var addr = customer?.Address;
                
        
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

    public override string? ToString()
    {
        return base.ToString();
    }

    public async Task UpdateUserProfile(UserProfileEdit newProfile, ClaimsPrincipal userPrincipal)
    {
        var user = await _userManager.GetUserAsync(userPrincipal)
            ?? throw new BackendException(404, "Requested user does not exist.");

        user.BirthDate = newProfile.BirthDate ?? user.BirthDate;
        user.FullName = newProfile.Name ?? user.FullName;
        user.Gender = newProfile.Gender ?? user.Gender;
        user.PhoneNumber = newProfile.PhoneNumber ?? user.PhoneNumber;

        if (newProfile.Address is not null)
        {
            Customer customer = await _dbcontext.Customers.FindAsync(user.Id)
                ?? throw new BackendException(
                    400,
                    "Can't assign address. You are not a customer?",
                    $"Address assigning error for user: {user.Id}"
                );
            customer.Address = newProfile.Address;
        }
        await _dbcontext.SaveChangesAsync();
        await _userManager.UpdateAsync(user);
    }
}