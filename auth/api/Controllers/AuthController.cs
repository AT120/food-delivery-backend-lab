using AuthApi.Models;
using AuthDAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthApi.Controllers;


[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{

    private readonly UserManager<User> _userManager;

    public AuthController(UserManager<User> um)
    {
        _userManager = um;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterCreds creds)
    {
        var user = new User {
            Email = creds.Email,
            FullName = creds.Name,
            PhoneNumber = creds.PhoneNumber
        };

        _userManager.CreateAsync()
        
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login() 
    {
        return Problem("This method has not been yet implemented", statusCode: 501); 
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout() 
    {
        return Problem("This method has not been yet implemented", statusCode: 501); 
    }
}
