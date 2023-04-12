using AuthBL;
using AuthCommon.DTO;
using AuthCommon.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjCommon;
using ProjCommon.Enums;
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
    [Authorize(Policies.RefreshOnly)]
    public async Task<ActionResult> Logout(bool AllUserTokens)
    {
        try
        {
            if (AllUserTokens)
                await _authService.Logout(
                    ClaimsHelper.GetValue<Guid>(ClaimType.UserId, User)
                );
            else
                await _authService.Logout(
                    ClaimsHelper.GetValue<int>(ClaimType.TokenId, User)
                );
            return NoContent();
        }
        catch
        {
            return Problem("Provided token is malformed.", statusCode: 400);
        }
    }

    [HttpPost("refresh")]
    [Authorize(Policies.RefreshOnly)]
    public async Task<ActionResult<TokenPair>> Refresh()
    {
        try
        {
            return await _authService.Refresh(User);
        }
        catch (BackendException be)
        {
            return Problem(be.UserMessage, statusCode: be.StatusCode);
        }
    } 

    [Authorize]
    [HttpPost("change-password")]
    public async Task<ActionResult> ChangePassword(PasswordPair passwords)
    {
        try
        {
            await _authService.ChangePassword(passwords, User);
            return Ok();
        }
        catch (BackendException be) //TODO: удобно, но не отображает сути
        {
            return Problem(be.UserMessage, statusCode: be.StatusCode);
        }
        catch
        {
            return Problem("Unknown error", statusCode: 500);
        }
    }
}
