using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;


namespace Backend.Controllers;

[ApiController]
[Route("/api/restaurants")]
public class RestaurantController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Collection<Restaurant>>> GetRestaurants(int page, int nameQuery)
    {
        return Problem("This method has not been yet implemented", statusCode: 501); 
        // Пустой список или 404, 204?
    }


    //А мож меню вместе с ресторанами возвращать?
    [HttpGet("{restaurantId}/menus")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Collection<Menu>>> GetMenus(Guid restaurantId)
    {
        return Problem("This method has not been yet implemented", statusCode: 501); 
    }

    [HttpGet("{restaurantId}/dishes")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Collection<Dish>>> GetDishes(
        Guid restaurantId,
        uint page,
        bool vegetarianOnly,
        [FromQuery] Collection<Guid> menus,
        SortingTypes sorting)
    {
        return Problem("This method has not been yet implemented", statusCode: 501); 
    }


    



}
