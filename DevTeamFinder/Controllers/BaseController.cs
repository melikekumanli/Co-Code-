using Microsoft.AspNetCore.Mvc;

namespace DevTeamFinder.Controllers;

public class BaseController : Controller
{
    protected bool IsLoggedIn()
    {
        return HttpContext.Session.GetInt32("UserId") != null;
    }

    protected IActionResult RedirectToLoginIfNotAuthenticated()
    {
        if (!IsLoggedIn())
        {
            return RedirectToAction("Login", "Account");
        }
        return null;
    }
}
