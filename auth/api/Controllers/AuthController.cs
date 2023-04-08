using AuthCommon.DTO;
using AuthCommon.Interfaces;
using AuthDAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthApi.Controllers;


[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{

    private readonly IAuthService _authService;

    public AuthController(IAuthService auth)
    {
        _authService = auth;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterCreds creds)
    {
        try
        {
            await _authService.Register(creds);
            return Problem("sehes", statusCode: 200);
        }
        catch
        {
            return Problem("sehes", statusCode: 500);
        }
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
