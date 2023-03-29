using Backend.Models;
using Microsoft.AspNetCore.Mvc;


namespace Backend.Controllers;

[ApiController]
[Route("/api/restaurants/{restaurantId}/dishes")]
public class DishesController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ICollection<DishShort>>> GetDishes(
        Guid restaurantId,
        uint page,
        bool vegetarianOnly,
        [FromQuery] ICollection<int> menus,
        [FromQuery] ICollection<DishCategory> categories,
        SortingTypes sorting)
    {
        return Problem("This method has not been yet implemented", statusCode: 501); 
    } 


    [HttpGet("{dishId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DishDetailed>> GetDishInfo(Guid restaurantId, Guid dishId)
    {
        return Problem("This method has not been yet implemented", statusCode: 501);
    }

}