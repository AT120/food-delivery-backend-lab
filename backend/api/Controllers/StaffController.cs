using System.Buffers;
using BackendBl;
using BackendCommon.DTO;
using BackendCommon.Enums;
using BackendCommon.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjCommon;
using ProjCommon.Enums;
using ProjCommon.Exceptions;

namespace BackendApi.Controllers;

[Route("/api/staff/orders")]
[ApiController]
public class StaffController : ControllerBase
{

    private readonly IStaffService _staffService;

    public StaffController(IStaffService ss)
    {
        _staffService = ss;
    }

    
    /// <summary>
    /// Получить заказы (повар и менеджер)
    /// </summary>
    /// <param name="status">Сумма статусов OrderStatus, по которым будет производиться фильтрация</param>
    [HttpGet()]
    [Authorize(Roles = "Cook, Manager")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status206PartialContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetOrders(
        int? page,
        int status,
        int? orderId,
        StaffOrderSortingTypes sorting) 
    {
        try
        {
            var res = await _staffService.GetOrders(
                page ?? 1,
                status,
                orderId,
                sorting,
                User
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
    /// Получить заказы (курьер)
    /// </summary>
    /// <param name="inDelivery">Если true, то в результате будут заказы, которые уже взяты запрашиваемым курьером</param>
    [HttpGet("in-delivery")]
    [Authorize(Roles = "Courier")] //TODO: константой откуда-нибудь
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status206PartialContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ICollection<CourierOrder>>> GetOrders(
        int? page,
        bool? inDelivery,
        StaffOrderSortingTypes sorting)
    {
        try
        {
            var res = await _staffService.GetCourierOrders(
                page ?? 1,
                inDelivery ?? false,
                sorting,
                ClaimsHelper.GetUserId(User)
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
    /// Взять заказ, или выставить ему следующий статус (повар и курьер)
    /// </summary>
    [HttpPost("{orderId}/next-status")]
    [Authorize(Roles = "Courier, Cook")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)] 
    [ProducesResponseType(StatusCodes.Status404NotFound)] 
    public async Task<ActionResult> AssignNextStatus(int orderId)
    {
        try
        {
            await _staffService.NextStatus(orderId, User); 
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
    /// Отменить заказ (курьер)
    /// </summary>
    [HttpDelete("{orderId}")]
    [Authorize(Roles = "Courier")]
    [ProducesResponseType(StatusCodes.Status204NoContent)] 
    [ProducesResponseType(StatusCodes.Status400BadRequest)] 
    [ProducesResponseType(StatusCodes.Status404NotFound)] 
    public async Task<ActionResult> CancelOrder(int orderId)
    {
        try
        {
            await _staffService.CancelOrder(orderId, ClaimsHelper.GetUserId(User)); 
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
