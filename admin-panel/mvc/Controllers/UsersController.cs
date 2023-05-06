using AdminCommon.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AdminPanel.Controllers;

public class UsersController : Controller
{

    private readonly IAdminUserService _userService;

    public UsersController(IAdminUserService aus)
    {
        _userService = aus;
    }

    [HttpGet]
    public async Task<ActionResult> Index(int? page)
    {
        try
        {
            var res = await _userService.GetUsers(page ?? 1);   
            return View(res);
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    
}