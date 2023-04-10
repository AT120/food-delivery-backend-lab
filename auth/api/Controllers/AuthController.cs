using AuthCommon.DTO;
using AuthCommon.Interfaces;
using Microsoft.AspNetCore.Mvc;
using ProjCommon.Exceptions;

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
    public async Task<ActionResult<TokenPair>> Register(RegisterCreds creds)
    {
        try
        {
            return await _authService.Register(creds);    
        }
        catch (BackendException be)
        {
            return Problem(be.UserMessage, statusCode: be.StatusCode ?? 500);
        }
        catch
        {
            return Problem("eprst", statusCode: 500);
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<TokenPair>> Login(LoginCreds creds) 
    {
        try
        {
            return await _authService.Login(creds);    
        }
        catch (BackendException be)
        {
            return Problem(be.UserMessage, statusCode: be.StatusCode ?? 500);
        }
        catch
        {
            return Problem("eprst", statusCode: 500);
        }
    }


    [HttpPost("logout")]
    public async Task<IActionResult> Logout() 
    {
        return Problem("This method has not been yet implemented", statusCode: 501); 
    }
}
