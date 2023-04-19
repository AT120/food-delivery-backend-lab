using BackendBl;
using BackendCommon.DTO;
using BackendCommon.Enums;
using BackendCommon.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjCommon.Exceptions;

namespace BackendApi.Controllers;

[ApiController]
[Route("/api/restaurants/dishes")]
public class DishesController : ControllerBase
{

    private readonly IDishService _dishService;
    public DishesController(IDishService ds)
    {
        _dishService = ds;
    }
    

    [HttpGet("/api/restaurants/{restaurantId}/dishes")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ICollection<DishShort>>> GetDishes(
        Guid restaurantId,
        int? page,
        bool? vegetarianOnly,
        [FromQuery] ICollection<int>? menus,
        [FromQuery] ICollection<DishCategory>? categories,
        SortingTypes? sorting)
    {
        try
        {
            var res =  await _dishService.GetDishes(
                restaurantId,
                page ?? 1,
                vegetarianOnly ?? false,
                menus,
                categories,
                sorting ?? SortingTypes.PriceAsc
            );
            var pageInfo = res.PageInfo;
            Response.Headers.ContentRange = PaginationHelper.FillContentRange(pageInfo); 
            int statusCode = PaginationHelper.GetStatusCode(pageInfo);
            return StatusCode(statusCode, res.Items);
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


    [HttpGet("{dishId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DishDetailed>> GetDishInfo(Guid dishId)
    {   
        try
        {
            return await _dishService.GetDish(dishId, User);
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

    [HttpPut("{dishId}/rating")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> RateDish(Guid dishId, Rating rating)
    {
        try
        {
            await _dishService.RateDish(dishId, User, rating.value);
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