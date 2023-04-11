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
        }; //TODO: address
    }
}