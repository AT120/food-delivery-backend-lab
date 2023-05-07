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

    public UsersController(IAdminUserService aus)
    {
        _userService = aus;
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


    
}