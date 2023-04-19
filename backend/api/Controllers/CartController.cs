using BackendCommon.DTO;
using BackendCommon.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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


    [HttpGet]
    [Authorize]
    public async Task<ActionResult<CartDTO>> GetCart()
    {
        try
        {
            return await _cartService.GetCart(User);
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


    [HttpPost("dishes")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> PutDishToCart(DishCount dishCount)
    {
        try
        {
            await _cartService.PutDishIntoCart(dishCount, User);
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


    [HttpPut("dishes/{dishId}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> ChangeDishQuantity(Guid dishId, IntOnly newCount)
    {
        try
        {
            await _cartService.ChangeDishQunatity(dishId, User, newCount.value);
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


    [HttpDelete("dishes/{dishId}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> DeleteDish(Guid dishId)
    {
        try
        {
            await _cartService.DeleteDishFromCart(dishId, User);
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


    [HttpDelete("dishes")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CleanCart()
    {
        try
        {
            await _cartService.CleanCart(User);
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
