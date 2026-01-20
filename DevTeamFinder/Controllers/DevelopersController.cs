using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DevTeamFinder.Data;
using DevTeamFinder.Models;

namespace DevTeamFinder.Controllers;

public class DevelopersController : BaseController
{
    private readonly AppDbContext _context;
    private readonly ILogger<DevelopersController> _logger;

    public DevelopersController(AppDbContext context, ILogger<DevelopersController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        // Login kontrolü
        var redirectResult = RedirectToLoginIfNotAuthenticated();
        if (redirectResult != null)
        {
            return redirectResult;
        }

        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        // User → Developer'ı bul (DeveloperSkills include et)
        var developer = await _context.Developers
            .Include(d => d.DeveloperSkills)
                .ThenInclude(ds => ds.Skill)
            .FirstOrDefaultAsync(d => d.UserId == userId.Value);

        if (developer == null)
        {
            _logger.LogWarning("Developer bulunamadı: UserId={UserId}", userId);
            return RedirectToAction("Login", "Account");
        }

        // Tüm skill'leri listele (developer'ın sahip olmadıkları)
        var allSkills = await _context.Skills
            .OrderBy(s => s.Ad)
            .ToListAsync();

        var developerSkillIds = developer.DeveloperSkills?.Select(ds => ds.SkillId).ToList() ?? new List<int>();
        var availableSkills = allSkills.Where(s => !developerSkillIds.Contains(s.Id)).ToList();

        ViewData["AvailableSkills"] = new SelectList(availableSkills, "Id", "Ad");
        ViewData["Title"] = "Profilim - Yetenekler";

        return View(developer);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddSkill(int skillId)
    {
        // Login kontrolü
        var redirectResult = RedirectToLoginIfNotAuthenticated();
        if (redirectResult != null)
        {
            return redirectResult;
        }

        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        try
        {
            // User → Developer'ı bul
            var developer = await _context.Developers
                .FirstOrDefaultAsync(d => d.UserId == userId.Value);

            if (developer == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Skill var mı kontrol et
            var skill = await _context.Skills
                .FirstOrDefaultAsync(s => s.Id == skillId);

            if (skill == null)
            {
                TempData["ErrorMessage"] = "Seçilen yetenek bulunamadı.";
                return RedirectToAction("Index");
            }

            // Bu developer'ın bu skill'i zaten var mı kontrol et
            var existingSkill = await _context.DeveloperSkills
                .FirstOrDefaultAsync(ds => ds.DeveloperId == developer.Id && ds.SkillId == skillId);

            if (existingSkill != null)
            {
                TempData["ErrorMessage"] = "Bu yeteneğe zaten sahipsiniz.";
                return RedirectToAction("Index");
            }

            // DeveloperSkill oluştur
            var developerSkill = new DeveloperSkill
            {
                DeveloperId = developer.Id,
                SkillId = skillId
            };

            _context.DeveloperSkills.Add(developerSkill);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Yetenek eklendi: DeveloperId={DeveloperId}, SkillId={SkillId}", 
                developer.Id, skillId);

            TempData["SuccessMessage"] = $"'{skill.Ad}' yeteneği başarıyla eklendi.";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Yetenek ekleme sırasında hata oluştu: SkillId={SkillId}", skillId);
            TempData["ErrorMessage"] = "Yetenek eklenirken bir hata oluştu.";
            return RedirectToAction("Index");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveSkill(int skillId)
    {
        // Login kontrolü
        var redirectResult = RedirectToLoginIfNotAuthenticated();
        if (redirectResult != null)
        {
            return redirectResult;
        }

        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        try
        {
            // User → Developer'ı bul
            var developer = await _context.Developers
                .FirstOrDefaultAsync(d => d.UserId == userId.Value);

            if (developer == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // DeveloperSkill'i bul
            var developerSkill = await _context.DeveloperSkills
                .Include(ds => ds.Skill)
                .FirstOrDefaultAsync(ds => ds.DeveloperId == developer.Id && ds.SkillId == skillId);

            if (developerSkill == null)
            {
                TempData["ErrorMessage"] = "Bu yeteneğe sahip değilsiniz.";
                return RedirectToAction("Index");
            }

            var skillName = developerSkill.Skill.Ad;

            // DeveloperSkill'i sil
            _context.DeveloperSkills.Remove(developerSkill);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Yetenek silindi: DeveloperId={DeveloperId}, SkillId={SkillId}", 
                developer.Id, skillId);

            TempData["SuccessMessage"] = $"'{skillName}' yeteneği başarıyla kaldırıldı.";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Yetenek silme sırasında hata oluştu: SkillId={SkillId}", skillId);
            TempData["ErrorMessage"] = "Yetenek kaldırılırken bir hata oluştu.";
            return RedirectToAction("Index");
        }
    }
}
