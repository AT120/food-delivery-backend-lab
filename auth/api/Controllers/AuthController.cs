using AuthBL;
using AuthCommon.DTO;
using AuthCommon.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjCommon;
using ProjCommon.Enums;
using ProjCommon.Exceptions;
using ProjCommon.Helpers;

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

    /// <summary>
    /// Регистрация
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TokenPair>> Register(RegisterUserData creds)
    {
        try
        {
            return Created("/api/auth/profile", await _authService.Register(creds));
        }
        catch (BackendException be)
        {
            return Problem(be.UserMessage, statusCode: be.StatusCode);
        }
        catch
        {
            return Problem("eprst", statusCode: 500);
        }
    }

    /// <summary>
    /// Вход
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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



    /// <summary>
    /// Выход
    /// </summary>
    /// <remarks>В авторизации требуется Refresh-токен</remarks>
    /// <param name="AllUserTokens">Удалить ли все токены пользователя (false по умолчанию)</param>
    [HttpPost("logout")]
    [Authorize(Policies.RefreshOnly)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

    /// <summary>
    /// Обновить токен
    /// </summary>
    /// <remarks>В авторизации требуется Refresh-токен</remarks>
    [HttpPost("refresh")]
    [Authorize(Policies.RefreshOnly)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
        catch
        {
            return Problem("Unknown error", statusCode: 500);
        }
    } 

    /// <summary>
    /// Сменить пароль
    /// </summary>
    [Authorize]
    [HttpPost("change-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> ChangePassword(PasswordPair passwords)
    {
        try
        {
            await _authService.ChangePassword(passwords, User);
            return Ok();
        }
        catch (BackendException be)
        {
            return Problem(be.UserMessage, statusCode: be.StatusCode);
        }
        catch
        {
            return Problem("Unknown error", statusCode: 500);
        }
    }
}
