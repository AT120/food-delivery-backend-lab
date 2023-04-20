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

    
    [HttpGet()]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOrders(
        int? page,
        int status, //TODO: дописать в сваггере как работает несколько статусов.
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

    [HttpGet("in-delivery")]
    [Authorize(Roles = "Courier")] //TODO: константой откуда-нибудь
    public async Task<ActionResult<ICollection<CourierOrderDTO>>> GetOrders(
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


    [HttpPost("{orderId}/next-status")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status202Accepted)] //TODO: а мож не
    [ProducesResponseType(StatusCodes.Status404NotFound)] 
    [ProducesResponseType(StatusCodes.Status409Conflict)] //TODO: а мож не
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

    [HttpDelete("{orderId}")]
    [Authorize] //TODO: role courier
    [ProducesResponseType(StatusCodes.Status204NoContent)] 
    [ProducesResponseType(StatusCodes.Status404NotFound)] 
    [ProducesResponseType(StatusCodes.Status409Conflict)] //TODO: а мож не
    public async Task<ActionResult> CancelOrder(int orderId)
    {
        try
        {
            await _staffService.CancelOrder(orderId, ClaimsHelper.GetUserId(User)); 
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
