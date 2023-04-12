using BackendCommon.DTO;
using BackendCommon.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using ProjCommon.Exceptions;

namespace BackendApi.Controllers;

[ApiController]
[Route("/api/restaurants")]
public class RestaurantController : ControllerBase
{

    private readonly IRestaurantService _restaurantService;

    public RestaurantController(IRestaurantService rs)
    {
        _restaurantService = rs;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status206PartialContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ICollection<RestaurantDTO>>> GetRestaurants(int? pageNum, string? nameQuery)
    {
        try
        {
            var page = await _restaurantService.GetRestaurants(pageNum ?? 1, nameQuery);
            Response.Headers.ContentRange =$"restaurants {page.RangeStart}-{page.RangeEnd}/{page.Size}";
            int statusCode = ((page.RangeEnd - page.RangeStart) < page.Size) ? 206 : 200;
            return StatusCode(statusCode, page.Restaurants);
            
            //TODO: Пустой список или 404, 204?
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


    //А мож меню вместе с ресторанами возвращать?
    [HttpGet("{restaurantId}/menus")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ICollection<Menu>>> GetMenus(Guid restaurantId)
    {
        return Problem("This method has not been yet implemented", statusCode: 501); 
    }
}
