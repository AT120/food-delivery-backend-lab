using BackendCommon.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjCommon.DTO;

namespace BackendApi.Controllers;

[ApiController]
[Route("/api/staff/dishes")]
[Authorize(Roles = "Manager")]
public class StaffDishesController : Controller
{
    [HttpGet]
    public async Task<ActionResult<Page<DishShort>>> GetDishes() //TODO: Доп поля у менджеров
    {
        return Problem("This method has not been yet implemented", statusCode: 501); 
    }


    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreateDish()
    {
        return Problem("This method has not been yet implemented", statusCode: 501); 
    }


    [HttpPut("{dishId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> EditDish(Guid dishId)
    {
        return Problem("This method has not been yet implemented", statusCode: 501); 
    }


    [HttpDelete("{dishId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> RemoveDish(Guid dishId)
    {
        return Problem("This method has not been yet implemented", statusCode: 501); 
    }
}