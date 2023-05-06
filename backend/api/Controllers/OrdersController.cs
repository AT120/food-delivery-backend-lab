using BackendBl;
using BackendCommon.DTO;
using BackendCommon.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjCommon;
using ProjCommon.Exceptions;
using ProjCommon.Helpers;

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

    /// <summary>
    /// Получить заказы
    /// </summary>
    /// <param name="status">Сумма статусов OrderStatus, по которым будет производиться фильтрация</param>
    /// <response code="200">В ответе вернулись все заказы</response>
    /// <response code="206">В ответе вернулась часть заказов</response>
    [HttpGet]
    [Authorize(Roles = "Customer")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status206PartialContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ICollection<CustomerOrderShort>>> GetOrders(
        int? page,
        DateTime? startDate,
        DateTime? endDate,          
        int status,
        int? orderIdQuery)
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

    /// <summary>
    /// Получить подробную информацию о заказе
    /// </summary>
    [HttpGet("{orderId}")]
    [Authorize(Roles = "Customer")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerDetailedOrder>> GetOrder(int orderId)
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

    /// <summary>
    /// Создать заказ из корзины
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Customer")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreateOrder(OrderInfo orderInfo)
    {
        try
        {
            int orderId = await _orderService.CreateOrderFromCart(
                ClaimsHelper.GetUserId(User),
                orderInfo.Address,
                orderInfo.DeliveryTime
            );
            return Created($"/api/orders/{orderId}", null);
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
    /// Повторить заказ
    /// </summary>
    /// <remarks>Доступные блюда из указанного заказа будут добавлены в корзину</remarks>
    [HttpPost("{orderId}/repeat")]
    [Authorize(Roles = "Customer")]
    [ProducesResponseType(StatusCodes.Status201Created)]
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
    /// Отменить заказ
    /// </summary>
    [HttpDelete("{orderId}")]
    [Authorize(Roles = "Customer")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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