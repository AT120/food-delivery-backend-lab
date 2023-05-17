using System.Buffers;
using AdminCommon.Interfaces;
using AuthDAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjCommon.Enums;
using ProjCommon.Exceptions;

namespace AdminBL.Services;

public class AdminAuthService : IAdminAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    public AdminAuthService(UserManager<User> um, SignInManager<User> sm)
    {
        _userManager = um;
        _signInManager = sm;
    }

    private readonly BackendException FailedLogin= new (401, "Неверный логин или пароль.");

    // public void SingIn()

    public async Task SignIn(string Email, string Password)
    {
        var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.Email == Email)
                ?? throw FailedLogin;

        var isUserAdmin = await _userManager.IsInRoleAsync(user, RoleType.Admin.ToString());
        if (!isUserAdmin)
            throw FailedLogin;

        var res = await _signInManager.PasswordSignInAsync(

            Email,
            Password,
            false,
            false
        );

        if (!res.Succeeded)
            throw FailedLogin;
    }
}