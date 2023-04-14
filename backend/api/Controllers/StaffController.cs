using BackendCommon.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendApi.Controllers;

[Route("/api/staff")]
[ApiController]
public class StaffController : ControllerBase
{
    [HttpGet("orders")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    // [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOrders(
        int? page,
        OrderStatus status, //TODO: дописать в сваггере как работает несколько статусов.
        int orderId
        //TODO: ну доделать тут серьезно надо ёмаё (сортировка)
        ) 
    {
        //COOK: все CREATED и KITCHEN, PACKAGING связанные с запрашивающим
        //MANAGER: все статусы
        return Ok();
    }

    [HttpPost("orders/{orderId}/next-status")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status202Accepted)] //TODO: а мож не
    [ProducesResponseType(StatusCodes.Status404NotFound)] 
    [ProducesResponseType(StatusCodes.Status409Conflict)] //TODO: а мож не
    public async Task<ActionResult> AssignNextStatus(int orderId)
    {
        return Problem("This method has not been yet implemented", statusCode: 501); 
    }

    [HttpDelete("orders/{orderId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)] 
    [ProducesResponseType(StatusCodes.Status404NotFound)] 
    [ProducesResponseType(StatusCodes.Status409Conflict)] //TODO: а мож не
    public async Task<ActionResult> CancelOrder(int orderId)
    {
        return Problem("This method has not been yet implemented", statusCode: 501); 
    }
    
}
