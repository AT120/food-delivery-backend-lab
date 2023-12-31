using AdminCommon.Interfaces;
using AdminPanel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjCommon.DTO;
using ProjCommon.Exceptions;

namespace AdminPanel.Controllers;

[Authorize(Roles = "Admin")]
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
        return View(restaurants);
    }

    [HttpGet]
    public async Task<ActionResult> ListRestaurants(int? page, string? searchQuery)
    {
        var restaurants = await _restaurantService.GetRestaurants(page ?? 1, searchQuery);
        return new JsonResult(restaurants);
    }

    [HttpPost]
    public async Task<ActionResult> Edit(GenericItem rest)
    {
        try
        {
            await _restaurantService.EditRestaurant(rest.Id, rest.Name);
            return View("Success");
        }
        catch (BackendException be)
        {
            var evm = new ErrorViewModel
            {
                Message = be.UserMessage
            };
            return View("Error", evm);
        }
        catch
        {
            return View("Error");
        }   
    }


    [HttpPost]
    public async Task<ActionResult> Delete(IdOnly rest)
    {
        try
        {
            await _restaurantService.DeleteRestaurant(rest.Id);
            return View("Success");
        }
        catch (BackendException be)
        {
            var evm = new ErrorViewModel
            {
                Message = be.UserMessage
            };
            return View("Error", evm);
        }
        catch
        {
            return View("Error");
        }   
    }


    [HttpPost]
    public async Task<ActionResult> Create(NameOnly rest)
    {
        try
        {
            await _restaurantService.CreateRestaurant(rest.Name);
            return View("Success");
        }
        catch (BackendException be)
        {
            var evm = new ErrorViewModel
            {
                Message = be.UserMessage
            };
            return View("Error", evm);
        }
        catch
        {
            return View("Error");
        }   
    }
}