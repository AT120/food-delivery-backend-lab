using AdminCommon.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AdminPanel.Controllers;

public class RestaurantController : Controller
{

    private readonly IAdminRestaurantService _restaurantService;
    public RestaurantController(IAdminRestaurantService rs)
    {
        _restaurantService = rs;
    }


    public async Task<ActionResult> Index(int? page, string? searchQuery)
    {
        var restaurants = await _restaurantService.GetRestaurants(page ?? 1, searchQuery);
        return View("List", restaurants);
    }


    // public async Task<ActionResult>
}