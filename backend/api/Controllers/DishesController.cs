using BackendBl;
using BackendCommon.DTO;
using BackendCommon.Enums;
using BackendCommon.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjCommon;
using ProjCommon.Exceptions;
using ProjCommon.Helpers;

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
    
    /// <summary>
    /// Получить блюда в заданном ресторане
    /// </summary>
    /// <response code="200">В ответе вернулись все блюда</response>
    /// <response code="206">В ответе вернулась часть блюд</response>
    [HttpGet("/api/restaurants/{restaurantId}/dishes")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status206PartialContent)]
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

    /// <summary>
    /// Получить подробную информацию о блюде
    /// </summary>
    /// <remarks>
    /// Если при запросе пользователь будет авторизован, то в ответе будут дополнительно отправлены:
    /// - возможность оставить оценку этому блюду у пользователя (canBeRated)
    /// - предыдущая оценка, оставленная эти пользователем (previousRating)
    /// </remarks>
    [HttpGet("{dishId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DishDetailed>> GetDishInfo(Guid dishId)
    {   
        try
        {
            return await _dishService.GetDish(dishId, ClaimsHelper.TryGetUserId(User));
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
    /// Поставить блюду оценку
    /// </summary>
    [HttpPut("{dishId}/rating")]
    [Authorize(Roles = "Customer")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> RateDish(Guid dishId, Rating rating)
    {
        try
        {
            await _dishService.RateDish(dishId, rating.value, ClaimsHelper.GetUserId(User));
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