using BackendBl;
using BackendCommon.DTO;
using BackendCommon.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using ProjCommon.DTO;
using ProjCommon.Exceptions;
using ProjCommon.Helpers;
using ProjCommon.Interfaces;

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


    /// <summary>
    /// Получить рестораны
    /// </summary>
    /// <response code="200">В ответе вернулись все рестораны</response>
    /// <response code="206">В ответе вернулась часть ресторанов</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status206PartialContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ICollection<Restaurant>>> GetRestaurants(int? pageNum, string? nameQuery)
    {
        try
        {
            var restaurantsPage = await _restaurantService.GetRestaurants(pageNum ?? 1, nameQuery);
            var page = restaurantsPage.PageInfo;
            Response.Headers.ContentRange = PaginationHelper.FillContentRange(page); 
            int statusCode = PaginationHelper.GetStatusCode(page);
            return StatusCode(statusCode, restaurantsPage.Items);
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
    /// Получить все меню указанного ресторана
    /// </summary>
    [HttpGet("{restaurantId}/menus")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ICollection<MenuDTO>>> GetMenus(Guid restaurantId)
    {
        try
        {
            return Ok(await _restaurantService.GetMenus(restaurantId));
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
