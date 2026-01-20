using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DevTeamFinder.Data;
using DevTeamFinder.Models;
using DevTeamFinder.ViewModels;
using System.Security.Cryptography;
using System.Text;

namespace DevTeamFinder.Controllers;

public class AccountController : Controller
{
    private readonly AppDbContext _context;
    private readonly ILogger<AccountController> _logger;

    public AccountController(AppDbContext context, ILogger<AccountController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Login()
    {
        ViewData["Title"] = "Giriş Yap";
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Title"] = "Giriş Yap";
            return View(model);
        }

        // Email küçük harfe çevrilerek User tablosunda ara
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == model.Email.ToLower());

        if (user == null)
        {
            // Güvenlik nedeniyle genel hata mesajı
            ModelState.AddModelError("", "E-posta veya şifre hatalı.");
            ViewData["Title"] = "Giriş Yap";
            return View(model);
        }

        // Girilen şifreyi SHA256 ile hash'le
        var hashedPassword = HashPassword(model.Sifre);

        // Hash == PasswordHash mi karşılaştır
        if (hashedPassword != user.PasswordHash)
        {
            // Güvenlik nedeniyle genel hata mesajı
            ModelState.AddModelError("", "E-posta veya şifre hatalı.");
            ViewData["Title"] = "Giriş Yap";
            return View(model);
        }

        // Başarılı giriş - Session oluştur
        HttpContext.Session.SetInt32("UserId", user.Id);
        HttpContext.Session.SetString("UserEmail", user.Email);

        _logger.LogInformation("Kullanıcı giriş yaptı: {Email}", user.Email);

        // Projects/Index'e yönlendir
        return RedirectToAction("Index", "Projects");
    }

    [HttpGet]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        _logger.LogInformation("Kullanıcı çıkış yaptı.");
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Register()
    {
        ViewData["Title"] = "Kayıt Ol";
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Title"] = "Kayıt Ol";
            return View(model);
        }

        // Email daha önce kayıtlı mı kontrol et
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == model.Email.ToLower());

        if (existingUser != null)
        {
            ModelState.AddModelError("Email", "Bu email adresi zaten kullanılıyor.");
            ViewData["Title"] = "Kayıt Ol";
            return View(model);
        }

        try
        {
            // Şifreyi hash'le
            var passwordHash = HashPassword(model.Sifre);

            // User oluştur
            var user = new User
            {
                Email = model.Email.ToLower(),
                PasswordHash = passwordHash,
                CreatedAt = DateTime.UtcNow
            };

            // User'ı veritabanına ekle
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Developer oluştur (UserId ile)
            var developer = new Developer
            {
                UserId = user.Id,
                AdSoyad = model.AdSoyad,
                Hakkinda = string.Empty,
                DeneyimSeviyesi = "Başlangıç"
            };

            // Developer'ı veritabanına ekle
            _context.Developers.Add(developer);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Yeni kullanıcı kaydedildi: {Email}", model.Email);

            // Başarılı kayıt sonrası login sayfasına yönlendir
            TempData["SuccessMessage"] = "Kayıt işlemi başarılı! Giriş yapabilirsiniz.";
            return RedirectToAction("Login");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Kullanıcı kaydı sırasında hata oluştu: {Email}", model.Email);
            ModelState.AddModelError("", "Kayıt işlemi sırasında bir hata oluştu. Lütfen tekrar deneyin.");
            ViewData["Title"] = "Kayıt Ol";
            return View(model);
        }
    }

    // SHA256 ile şifre hashleme
    private static string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}
