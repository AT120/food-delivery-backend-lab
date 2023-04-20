using BackendBl;
using BackendCommon.DTO;
using BackendCommon.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjCommon;
using ProjCommon.Exceptions;

namespace BackendApi.Controllers;

[Route("/api/orders")]
[ApiController]
public class OrdersController : ControllerBase
{

    private readonly IOrderService _orderService;
    public OrdersController(IOrderService os)
    {
        _orderService = os;
    }

    [HttpGet]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ICollection<CustomerOrderShortDTO>>> Get(
        int? page,
        DateTime? startDate,
        DateTime? endDate,          
        int status, //TODO: прописать в доку, что тут типо из OrderStatus
        int? orderIdQuery)
        //TODO: EnumSchemaFilter https://avarnon.medium.com/how-to-show-enums-as-strings-in-swashbuckle-aspnetcore-628d0cc271e6
    {
        try
        {
            var res =  await _orderService.GetOrders(
                ClaimsHelper.GetUserId(User),
                page ?? 1,
                startDate,
                endDate,
                status,
                orderIdQuery
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


    [HttpGet("{orderId}")]
    [Authorize] //TODO: role = customer
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CustomerDetailedOrderDTO>> GetOrder(int orderId)
    {
        try
        {
            return await _orderService.GetOrder(orderId, ClaimsHelper.GetUserId(User));
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

    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<int>> CreateOrder(OrderInfo orderInfo) //TODO: OrderId или int?
    {
        try
        {
            return await _orderService.CreateOrderFromCart(
                ClaimsHelper.GetUserId(User),
                orderInfo.Address,
                orderInfo.DeliveryTime
            );
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

    [HttpPost("{orderId}/repeat")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)] //TODO: остутствующие блюда
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> RepeatOrder(int orderId)
    {
        try
        {
            await _orderService.RepeatOrder(
                orderId,
                ClaimsHelper.GetUserId(User)
            );

            return Created("/api/cart", "test"); //TODO: убрать тест
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


    [HttpDelete("{orderId}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    //TODO: че возвращать если заказ уже на кухне?
    public async Task<ActionResult> CancelOrder(int orderId)
    {
        try
        {
            await _orderService.CancelOrder(
                orderId,
                ClaimsHelper.GetUserId(User)
            );

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