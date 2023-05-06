using AdminCommon.Interfaces;
using BackendDAL;
using Microsoft.AspNetCore.Mvc;

namespace AdminPanel.Controllers;

public class RestaurantController : Controller
{

    private readonly IAdminRestaurantService _restaurantService;

    public RestaurantController(IAdminRestaurantService rs)
    {
        _restaurantService = rs;
    }

    [HttpGet]
    public async Task<ActionResult> Index(int? page, string? searchQuery)
    {
        var restaurants = await _restaurantService.GetRestaurants(page ?? 1, searchQuery);
        return View("List", restaurants);
    }


    [HttpPost]
    public async Task<ActionResult> EditRestaurant()
    {
        return Problem("This method has not been yet implemented", statusCode: 501); 
    }
    // public async Task<ActionResult>
}