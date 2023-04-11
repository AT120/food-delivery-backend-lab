using AuthCommon.DTO;
using AuthCommon.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

    [Authorize()]
    [HttpGet]
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
    }


    [HttpPut]
    public async Task<ActionResult> UpdateUserProfile()
    {
        return Problem("This method has not been yet implemented", statusCode: 501); 
    }
    

}