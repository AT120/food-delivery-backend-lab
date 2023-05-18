using AdminCommon.Interfaces;
using AdminPanel.Models;
using AuthDAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjCommon.DTO;
using ProjCommon.Exceptions;

namespace AdminPanel.Controllers;

public class AuthController : Controller
{

    private readonly IAdminAuthService _authService;
    public AuthController(IAdminAuthService aus)
    {
        _authService = aus;
    }


    [HttpGet]
    public ActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<ActionResult> Login(LoginCreds creds, string? returnUrl)
    {
        returnUrl ??= "/";
        try
        {
            await _authService.SignIn(creds.Email, creds.Password);
            return LocalRedirect(returnUrl);
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
    public async Task<ActionResult> Logout()
    {
        await _authService.SignOut();
        return LocalRedirect("/");
    }
}