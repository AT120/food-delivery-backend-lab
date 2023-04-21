using BackendCommon.DTO;
using BackendCommon.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjCommon;
using ProjCommon.Exceptions;

namespace BackendApi.Controllers;

[ApiController]
[Route("/api/cart")]
public class CartController : ControllerBase
{

    private readonly ICartService _cartService;
    public CartController(ICartService cs)
    {
        _cartService = cs;
    }

    /// <summary>
    /// Получить корзину
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Customer")]
    public async Task<ActionResult<Cart>> GetCart()
    {
        try
        {
            return await _cartService.GetCart(ClaimsHelper.GetUserId(User));
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
    /// Добавить блюдо в корзину
    /// </summary>
    /// <response code="404">Блюда с заданным id не существует</response>
    [HttpPost("dishes")]
    [Authorize(Roles = "Customer")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> PutDishToCart(DishCount dishCount)
    {
        try
        {
            await _cartService.PutDishIntoCart(dishCount, ClaimsHelper.GetUserId(User));
            return Created("/api/cart", null);
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
    /// Изменить количество блюда в корзине
    /// </summary>
    /// <response code="404">Заданного блюда нет в корзине</response>
    [HttpPut("dishes/{dishId}")]
    [Authorize(Roles = "Customer")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> ChangeDishQuantity(Guid dishId, IntOnly newCount)
    {
        try
        {
            await _cartService.ChangeDishQunatity(dishId, newCount.value, ClaimsHelper.GetUserId(User));
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
    /// Удалить блюдо из корзины
    /// </summary>
    [HttpDelete("dishes/{dishId}")]
    [Authorize(Roles = "Customer")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> DeleteDish(Guid dishId)
    {
        try
        {
            await _cartService.DeleteDishFromCart(dishId, ClaimsHelper.GetUserId(User));
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
    /// Очистить корзину
    /// </summary>
    [HttpDelete("dishes")]
    [Authorize(Roles = "Customer")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CleanCart()
    {
        try
        {
            await _cartService.CleanCart(ClaimsHelper.GetUserId(User));
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
