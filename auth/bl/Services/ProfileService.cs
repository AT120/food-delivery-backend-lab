using AuthCommon.DTO;
using AuthDAL;
using AuthDAL.Models;
using Microsoft.AspNetCore.Identity;

namespace AuthBL.Services;

public class ProfileService
{
    private readonly UserManager<User> _userManager;
    private readonly AuthDBContext _dbcontext;
    private readonly SignInManager<User> _signInManager;
    private readonly RoleManager<Role> _roleManager;

    public ProfileService(
        UserManager<User> um,
        SignInManager<User> sim,
        AuthDBContext dbc,
        RoleManager<Role> rm)
    {
        _userManager = um;
        _signInManager = sim;
        _dbcontext = dbc;
        _roleManager = rm;
    }

    public async Task<UserProfile> GetUserProfile()
    {
        return new UserProfile();
    }
}