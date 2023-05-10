using AdminCommon.DTO;
using AdminCommon.Interfaces;
using AdminPanel.Models;
using Microsoft.AspNetCore.Mvc;
using ProjCommon.Enums;
using ProjCommon.Exceptions;

namespace AdminPanel.Controllers;

public class UsersController : Controller
{

    private readonly IAdminUserService _userService;
    private readonly IAdminRestaurantService _restaurantService;

    public UsersController(
        IAdminUserService aus,
        IAdminRestaurantService ars)
    {
        _userService = aus;
        _restaurantService = ars;
    }

    [HttpGet]
    public async Task<ActionResult> Index(
        int? page,
        string? nameSearchQuery,
        string? emailSearchQuery,
        string phoneSearchQuery,
        Gender? gender,
        IEnumerable<RoleType>? roles)
    {
        try
        {
            var res = await _userService.GetUsers(
                page ?? 1,
                nameSearchQuery,
                emailSearchQuery,
                phoneSearchQuery,
                gender,
                roles
            );

               
            return View(res);
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    [HttpGet]
    public async Task<ActionResult> EditUserPage(Guid userId)
    {
        try
        {
            var user = await _userService.GetUser(userId);
            ViewData["InputModel"] = user;
            return View();
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
    public async Task<ActionResult> EditUser(UserProfileEditDTO editModel)
    {
        try
        {
            await _userService.EditUser(new UserProfileEdit(editModel));
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

    [HttpGet]
    public async Task<ActionResult> NewUserPage()
    {
        ViewData["Restaurants"] = await _restaurantService.GetAvailableRestaurants();
        return View();
    }

    [HttpPost]
    public async Task<ActionResult> CreateUser(UserProfileCreateDTO user)
    {   
        try
        {
            await _userService.CreateUser(new UserProfileCreate(user));
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
    public async Task<ActionResult> Delete(UserId user)
    {
        try
        {
            await _userService.DeleteUser(user.Id);
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