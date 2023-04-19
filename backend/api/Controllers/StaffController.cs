using BackendCommon.DTO;
using BackendCommon.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjCommon;
using ProjCommon.Enums;

namespace BackendApi.Controllers;

[Route("/api/staff/orders")]
[ApiController]
public class StaffController : ControllerBase
{
    [HttpGet()]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    // [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOrders(
        int? page,
        OrderStatus status, //TODO: дописать в сваггере как работает несколько статусов.
        int orderId,
        StaffOrderSortingTypes sorting) 
    {
        return Ok();
    }

    [HttpGet("in-delivery")]
    [Authorize(Roles = "Courier")] //TODO: константой откуда-нибудь
    public async Task<ActionResult<ICollection<CourierOrderDTO>>> GetOrders(
        int? page,
        bool? inDelivery,
        StaffOrderSortingTypes sorting)
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


    [HttpPost("{orderId}/next-status")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status202Accepted)] //TODO: а мож не
    [ProducesResponseType(StatusCodes.Status404NotFound)] 
    [ProducesResponseType(StatusCodes.Status409Conflict)] //TODO: а мож не
    public async Task<ActionResult> AssignNextStatus(int orderId)
    {
        return Problem("This method has not been yet implemented", statusCode: 501); 
    }

    [HttpDelete("{orderId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)] 
    [ProducesResponseType(StatusCodes.Status404NotFound)] 
    [ProducesResponseType(StatusCodes.Status409Conflict)] //TODO: а мож не
    public async Task<ActionResult> CancelOrder(int orderId)
    {
        return Problem("This method has not been yet implemented", statusCode: 501); 
    }
    
}
