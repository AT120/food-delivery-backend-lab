using AuthCommon.DTO;
using AuthCommon.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjCommon.DTO;
using ProjCommon.Exceptions;

namespace AuthApi.Controllers;

[Route("/api/profile")]
[ApiController]
public class ProfileController : ControllerBase
{

    private readonly IProfileService _profileService;
    public ProfileController(IProfileService ps)
    {
        _profileService = ps;
    }

    /// <summary>
    /// Получить свой профиль
    /// </summary>
    [Authorize]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserProfile>> GetUserProfile()
    {
        try
        {
            return await _profileService.GetUserProfile(User); 
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
    /// Отредактировать свой профиль
    /// </summary>
    [Authorize]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> UpdateUserProfile(UserProfileEdit profile)
    {
        try
        {
            await _profileService.UpdateUserProfile(profile, User);
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