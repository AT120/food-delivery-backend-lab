using System.Threading.Tasks;
using AuthCommon.DTO;
using AuthCommon.Interfaces;
using AuthDAL.Models;
using Microsoft.AspNetCore.Identity;

namespace AuthBL.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public AuthService(UserManager<User> um, SignInManager<User> sim)
    {
        _userManager = um;
        _signInManager = sim;
    }
    
    public async Task Register(RegisterCreds creds)
    {
        var user = new User(creds);
        var res = await _userManager.CreateAsync(user, creds.Passwrod);
        if (res.Succeeded)
        {
            // _userManager.AddClaimsAsync()
        }
        
    }

    public async Task Login(LoginCreds creds)
    {

    }
}