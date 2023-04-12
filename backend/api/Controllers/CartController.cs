using BackendCommon.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace BackendApi.Controllers;

[ApiController]
[Route("/api/cart")]
public class CartController : ControllerBase
{
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<Cart>> GetCart()
    {
        return Problem("This method has not been yet implemented", statusCode: 501); 
    }


    [HttpPost("dishes")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> PutDishToCart(DishCount id)
    {
        return Problem("This method has not been yet implemented", statusCode: 501); 
    }


    [HttpPut("dishes/{dishId}")]
    [Authorize]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> ChangeDishQuantity(Guid dishId, IntOnly delta)
    {
        return Problem("This method has not been yet implemented", statusCode: 501); 
    }


    [HttpDelete("dishes/{dishId}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteDish(Guid dishId)
    {
        return Problem("This method has not been yet implemented", statusCode: 501); 
    }

}
