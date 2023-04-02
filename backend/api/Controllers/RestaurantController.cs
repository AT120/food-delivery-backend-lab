using BackendApi.Models;
using Microsoft.AspNetCore.Mvc;


namespace BackendApi.Controllers;

[ApiController]
[Route("/api/restaurants")]
public class RestaurantController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ICollection<Restaurant>>> GetRestaurants(int page, int nameQuery)
    {
        return Problem("This method has not been yet implemented", statusCode: 501); 
        // Пустой список или 404, 204?
    }


    //А мож меню вместе с ресторанами возвращать?
    [HttpGet("{restaurantId}/menus")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ICollection<Menu>>> GetMenus(Guid restaurantId)
    {
        return Problem("This method has not been yet implemented", statusCode: 501); 
    }
}
