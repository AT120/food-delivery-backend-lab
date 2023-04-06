using Microsoft.AspNetCore.Mvc;

namespace AuthApi.Controllers;

[ApiController]
[Route("/api/profile")]
public class ProfileController : ControllerBase
{

    [HttpGet]
    public async Task<ActionResult> GetUserProfile()
    {
        return Problem("This method has not been yet implemented", statusCode: 501); 
    }

    [HttpPut]
    public async Task<ActionResult> UpdateUserProfile()
    {
        return Problem("This method has not been yet implemented", statusCode: 501); 
    }
    

}