using AuthCommon.DTO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthApi.Controllers;

[Route("/api/profile")]
[ApiController]
public class ProfileController : ControllerBase
{

    [Authorize()]
    [HttpGet]
    public async Task<ActionResult<UserProfile>> GetUserProfile()
    {
        return Problem("This method has not been yet implemented", statusCode: 501); 
    }


    [HttpPut]
    public async Task<ActionResult> UpdateUserProfile()
    {
        return Problem("This method has not been yet implemented", statusCode: 501); 
    }
    

}