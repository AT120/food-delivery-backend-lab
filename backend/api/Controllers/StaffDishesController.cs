using BackendCommon.DTO;
using BackendCommon.Enums;
using BackendCommon.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjCommon.DTO;
using ProjCommon.Exceptions;
using ProjCommon.Helpers;

namespace BackendApi.Controllers;

[ApiController]
[Route("/api/staff/dishes")]
[Authorize(Roles = "Manager")]
public class StaffDishesController : Controller
{

    private readonly IDishService _dishService;
    public StaffDishesController(IDishService ds)
    {
        _dishService = ds;
    }

    /// <summary>
    /// Получить блюда в ресторане
    /// </summary>
    /// <response code="200">В ответе вернулись все блюда</response>
    /// <response code="206">В ответе вернулась часть блюд</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status206PartialContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Page<DishShort>>> GetDishes(
        int? page,
        bool? vegetarianOnly,
        [FromQuery] ICollection<int>? menus,
        [FromQuery] ICollection<DishCategory>? categories,
        SortingTypes? sorting,
        bool? archived)
    {
        try
        {
            var res = await _dishService.GetDishesManager(
                    ClaimsHelper.GetUserId(User),
                    page ?? 1,
                    vegetarianOnly ?? false,
                    menus,
                    categories,
                    sorting ?? SortingTypes.PriceAsc,
                    archived
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
    /// Добавить новое блюдо
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreateDish(DishCreate dish)
    {
        try
        {
            Guid dishId = await _dishService.CreateDish(ClaimsHelper.GetUserId(User), dish);
            return Created($"/api/restaurants/dishes/{dishId}", null);
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
    /// Изменить блюдо (в том числе заархивировать)
    /// </summary>
    [HttpPut("{dishId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> EditDish(Guid dishId, DishEdit dish)
    {
        try
        {
            await _dishService.EditDish(dishId,ClaimsHelper.GetUserId(User), dish);
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