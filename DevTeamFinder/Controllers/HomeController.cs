using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DevTeamFinder.Models;

namespace DevTeamFinder.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        // Giriş yapmış kullanıcıyı Projelerim'e yönlendir
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId != null)
        {
            return RedirectToAction("Index", "Projects");
        }
        
        // Giriş yapmamış kullanıcıyı Login'e yönlendir
        return RedirectToAction("Login", "Account");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
