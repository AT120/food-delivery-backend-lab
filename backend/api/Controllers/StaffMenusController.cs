using BackendCommon.DTO;
using BackendCommon.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjCommon.DTO;
using ProjCommon.Exceptions;
using ProjCommon.Helpers;

namespace BackendApi.Controllers;

[ApiController]
[Route("/api/staff/menus")]
[Authorize(Roles = "Manager")]
public class StaffMenusController : Controller
{

    private readonly IStaffMenuService _menuService;
    public StaffMenusController(IStaffMenuService ms)
    {
        _menuService = ms;
    }

    /// <summary> 
    /// Получить все меню
    /// </summary> 
    /// <response code="200">В ответе вернулись все блюда</response>
    /// <response code="206">В ответе вернулась часть блюд</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status206PartialContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Page<GenericItem>>> GetMenus(int? page, bool? archived)
    {
        try
        {
            var res = await _menuService.GetMenus(
                page ?? 1,
                archived,
                ClaimsHelper.GetUserId(User)
            );
            Response.Headers.ContentRange = PaginationHelper.FillContentRange(res.PageInfo);
            int statusCode = PaginationHelper.GetStatusCode(res.PageInfo);
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
    /// Создать новое меню
    /// </summary> 
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreateMenu(NewMenu menu)
    {
        try
        {
            int menuId = await _menuService.CreateMenu(
                menu.Name,
                menu.Dishes,
                ClaimsHelper.GetUserId(User)
            );
            return Created($"/api/staff/menus/{menuId}", null);
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
    /// Изменить меню
    /// </summary> 
    [HttpPut("{menuId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> EditMenu(int menuId, MenuEdit menu)
    {
        try
        {
            await _menuService.EditMenu(
                menuId,
                menu.Archive,
                menu.Name,
                ClaimsHelper.GetUserId(User)
            );
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


    /// <summary> 
    /// Удалить меню
    /// </summary> 
    [HttpDelete("{menuId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteMenu(int menuId)
    {
        try
        {
            await _menuService.DeleteMenu(menuId, ClaimsHelper.GetUserId(User));
            return NoContent();
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
    /// Получить подробную информацию о меню
    /// </summary> 
    [HttpGet("{menuId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MenuDetailed>> GetMenuDetailed(int menuId)
    {
        try
        {
            return await _menuService.GetMenuDetailed(menuId, ClaimsHelper.GetUserId(User));
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
    /// Добавить блюдо в меню
    /// </summary> 
    [HttpPost("{menuId}/dishes")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> AddDishToMenu(int menuId, IdOnly dish)
    {
        try
        {
            await _menuService.AddDishToMenu(menuId, dish.Id, ClaimsHelper.GetUserId(User));
            return Created($"/api/staff/menus/{menuId}", null);
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
    /// Удалить блюдо из меню
    /// </summary> 
    [HttpDelete("{menuId}/dishes/{dishId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteDishFromMenu(int menuId, Guid dishId)
    {
        try
        {
            await _menuService.DeleteDishFromMenu(menuId, dishId, ClaimsHelper.GetUserId(User));
            return NoContent();
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