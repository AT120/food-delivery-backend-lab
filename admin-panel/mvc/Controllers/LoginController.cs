using Microsoft.AspNetCore.Mvc;

namespace AdminPanel.Controllers;

public class LoginController : Controller
{
    public ActionResult Index()
    {
        return View();
    }
}