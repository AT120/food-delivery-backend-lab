using BackendCommon.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace BackendApi.Controllers;

[Route("/api/orders")]
[ApiController]
public class OrdersController : ControllerBase
{
    [HttpGet]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ICollection<Order>>> Get(
        int? page,
        DateTime? startDate,
        DateTime? endDate,
        OrderFilterStatus? status, //TODO: EnumSchemaFilter https://avarnon.medium.com/how-to-show-enums-as-strings-in-swashbuckle-aspnetcore-628d0cc271e6
        int? orderId)
    {
        return Problem("This method has not been yet implemented", statusCode: 501);
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OrderId>> CreateOrder()
    {
        return Problem("This method has not been yet implemented", statusCode: 501); 
    }

    [HttpPost("{orderId}/repeat")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)] //TODO: остутствующие блюда
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> RepeatOrder(int orderId)
    {
        return Problem("This method has not been yet implemented", statusCode: 501); 
    }


    [HttpDelete("{orderId}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)] //TODO: 409 instead?
    //TODO: че возвращать если заказ уже на кухне?
    public async Task<ActionResult> CancelOrder(int orderId)
    {
        return Problem("This method has not been yet implemented", statusCode: 501); 
    }

}