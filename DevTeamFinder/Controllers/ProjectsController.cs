using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DevTeamFinder.Data;
using DevTeamFinder.Models;

namespace DevTeamFinder.Controllers;

public class ProjectsController : BaseController
{
    private readonly AppDbContext _context;
    private readonly ILogger<ProjectsController> _logger;

    public ProjectsController(AppDbContext context, ILogger<ProjectsController> logger)
    {
        _context = context;
        _logger = logger;
    }

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

        // User → Developer'ı bul
        var developer = await _context.Developers
            .FirstOrDefaultAsync(d => d.UserId == userId.Value);

        if (developer == null)
        {
            _logger.LogWarning("Developer bulunamadı: UserId={UserId}", userId);
            return RedirectToAction("Login", "Account");
        }

        // SkillId filtresi varsa uygula
        var skillIdParam = Request.Query["skillId"].ToString();
        int? skillId = null;
        if (!string.IsNullOrEmpty(skillIdParam) && int.TryParse(skillIdParam, out int parsedSkillId))
        {
            skillId = parsedSkillId;
        }

        // Sadece login olan kullanıcının aktif projeleri listelenir
        // Kullanıcı ya proje sahibidir ya da projeye katılmıştır (KabulEdildi durumundaki davetleri olan)
        var query = _context.Projects
            .Include(p => p.ProjectSkills)
                .ThenInclude(ps => ps.Skill)
            .Include(p => p.Invitations)
            .Where(p => p.IsActive == true && (
                p.DeveloperId == developer.Id || // Proje sahibi
                p.Invitations.Any(i => i.DeveloperId == developer.Id && i.Durum == "KabulEdildi") // Katıldığı projeler
            ))
            .AsQueryable();

        // SkillId filtresi varsa uygula
        if (skillId.HasValue)
        {
            query = query.Where(p => p.ProjectSkills.Any(ps => ps.SkillId == skillId.Value));
        }

        var projects = await query
            .OrderByDescending(p => p.Id)
            .ToListAsync();

        // Skill listesi filtreleme için
        var skills = await _context.Skills
            .OrderBy(s => s.Ad)
            .ToListAsync();

        ViewData["Skills"] = new SelectList(skills, "Id", "Ad", skillId);
        ViewData["Title"] = "Projelerim";
        ViewData["SelectedSkillId"] = skillId;
        ViewData["CurrentDeveloperId"] = developer.Id; // Proje sahibi kontrolü için
        return View(projects);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        // Login kontrolü
        var redirectResult = RedirectToLoginIfNotAuthenticated();
        if (redirectResult != null)
        {
            return redirectResult;
        }

        // Skill listesini view'a gönder
        var skills = await _context.Skills
            .OrderBy(s => s.Ad)
            .ToListAsync();

        ViewData["Skills"] = skills;
        ViewData["Title"] = "Yeni Proje Oluştur";
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string baslik, string aciklama, int[] skillIds)
    {
        // Login kontrolü
        var redirectResult = RedirectToLoginIfNotAuthenticated();
        if (redirectResult != null)
        {
            return redirectResult;
        }

        // Basit validasyon
        if (string.IsNullOrWhiteSpace(baslik))
        {
            ModelState.AddModelError("baslik", "Proje başlığı zorunludur.");
            
            // Skill listesini tekrar view'a gönder
            var skills = await _context.Skills
                .OrderBy(s => s.Ad)
                .ToListAsync();
            ViewData["Skills"] = skills;
            ViewData["Title"] = "Yeni Proje Oluştur";
            return View();
        }

        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        try
        {
            // Session'dan UserId al
            // User → Developer'ı bul
            var developer = await _context.Developers
                .FirstOrDefaultAsync(d => d.UserId == userId.Value);

            if (developer == null)
            {
                _logger.LogWarning("Developer bulunamadı: UserId={UserId}", userId);
                return RedirectToAction("Login", "Account");
            }

            // Project oluştur (IsActive = true varsayılan)
            var project = new Project
            {
                Baslik = baslik,
                Aciklama = aciklama ?? string.Empty,
                Durum = "Aktif",
                DeveloperId = developer.Id,
                IsActive = true
            };

            // DB'ye kaydet (önce project'i kaydet ki Id alınsın)
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            // Seçilen skill'leri ProjectSkill tablosuna ekle
            if (skillIds != null && skillIds.Length > 0)
            {
                foreach (var skillId in skillIds)
                {
                    var projectSkill = new ProjectSkill
                    {
                        ProjectId = project.Id,
                        SkillId = skillId
                    };
                    _context.ProjectSkills.Add(projectSkill);
                }
                await _context.SaveChangesAsync();
            }

            _logger.LogInformation("Yeni proje oluşturuldu: {Baslik}, DeveloperId={DeveloperId}, SkillCount={SkillCount}", 
                baslik, developer.Id, skillIds?.Length ?? 0);

            // Projects/Index'e yönlendir
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Proje oluşturma sırasında hata oluştu: {Baslik}", baslik);
            ModelState.AddModelError("", "Proje oluşturulurken bir hata oluştu. Lütfen tekrar deneyin.");
            
            // Skill listesini tekrar view'a gönder
            var skills = await _context.Skills
                .OrderBy(s => s.Ad)
                .ToListAsync();
            ViewData["Skills"] = skills;
            ViewData["Title"] = "Yeni Proje Oluştur";
            return View();
        }
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
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

        // Projeyi bul (Invitations ve Developer include et) - Sadece aktif projeler
        var project = await _context.Projects
            .Include(p => p.Developer)
                .ThenInclude(d => d.User)
            .Include(p => p.ProjectSkills)
                .ThenInclude(ps => ps.Skill)
            .Include(p => p.Invitations)
                .ThenInclude(i => i.Developer)
                    .ThenInclude(d => d.User)
            .Where(p => p.Id == id && p.IsActive == true)
            .FirstOrDefaultAsync();

        if (project == null)
        {
            return NotFound();
        }

        // User → Developer'ı bul
        var currentDeveloper = await _context.Developers
            .FirstOrDefaultAsync(d => d.UserId == userId.Value);

        if (currentDeveloper == null)
        {
            return RedirectToAction("Login", "Account");
        }

        // Proje sahibi mi kontrolü
        bool isOwner = project.DeveloperId == currentDeveloper.Id;

        // Her geliştirici için davet durumunu al
        var developerInvitations = await _context.Invitations
            .Where(i => i.ProjectId == project.Id)
            .ToListAsync();

        // Geliştirici-Davet durumu dictionary'si
        var developerInvitationMap = developerInvitations
            .ToDictionary(i => i.DeveloperId, i => i.Durum);

        // Giriş yapan developer'ın bu projeye daveti var mı?
        bool hasInvitation = developerInvitationMap.ContainsKey(currentDeveloper.Id);
        string currentInvitationStatus = hasInvitation ? developerInvitationMap[currentDeveloper.Id] : null;

        // Tüm geliştiricileri listele (proje sahibi hariç ve aktif olanlar) - DeveloperSkills include et
        var allDevelopers = await _context.Developers
            .Include(d => d.User)
            .Include(d => d.DeveloperSkills)
                .ThenInclude(ds => ds.Skill)
            .Where(d => d.Id != project.DeveloperId && d.IsActive == true)
            .ToListAsync();

        // Geliştirici listesini ve davet durumlarını view'a gönder
        var developersWithInvitationStatus = allDevelopers.Select(d => new
        {
            Developer = d,
            InvitationStatus = developerInvitationMap.ContainsKey(d.Id) 
                ? developerInvitationMap[d.Id] 
                : null
        }).ToList();

        // Projeye katılan geliştiriciler (KabulEdildi durumundaki davetler) - DeveloperSkills include et
        var acceptedInvitationIds = project.Invitations
            .Where(i => i.Durum == "KabulEdildi")
            .Select(i => i.DeveloperId)
            .ToList();
        
        var projectTeamMembers = await _context.Developers
            .Include(d => d.User)
            .Include(d => d.DeveloperSkills)
                .ThenInclude(ds => ds.Skill)
            .Where(d => acceptedInvitationIds.Contains(d.Id))
            .ToListAsync();

        // Proje skill'lerini al
        var projectSkillIds = project.ProjectSkills.Select(ps => ps.SkillId).ToList();
        var projectSkillCount = projectSkillIds.Count;

        // Her geliştirici için uyum skoru hesapla
        var developersWithCompatibility = allDevelopers.Select(d =>
        {
            int compatibilityScore = 0;
            if (projectSkillCount > 0 && d.DeveloperSkills != null && d.DeveloperSkills.Any())
            {
                var developerSkillIds = d.DeveloperSkills.Select(ds => ds.SkillId).ToList();
                var commonSkills = projectSkillIds.Intersect(developerSkillIds).Count();
                compatibilityScore = (int)Math.Round((double)commonSkills / projectSkillCount * 100);
            }
            return new
            {
                Developer = d,
                CompatibilityScore = compatibilityScore
            };
        }).ToList();

        // Proje ekibi için uyum skoru hesapla
        var teamMembersWithCompatibility = new List<dynamic>();
        foreach (var member in projectTeamMembers)
        {
            // DeveloperSkills'i yükle
            var memberWithSkills = await _context.Developers
                .Include(d => d.DeveloperSkills)
                    .ThenInclude(ds => ds.Skill)
                .FirstOrDefaultAsync(d => d.Id == member.Id);

            int compatibilityScore = 0;
            if (projectSkillCount > 0 && memberWithSkills?.DeveloperSkills != null && memberWithSkills.DeveloperSkills.Any())
            {
                var developerSkillIds = memberWithSkills.DeveloperSkills.Select(ds => ds.SkillId).ToList();
                var commonSkills = projectSkillIds.Intersect(developerSkillIds).Count();
                compatibilityScore = (int)Math.Round((double)commonSkills / projectSkillCount * 100);
            }

            teamMembersWithCompatibility.Add(new
            {
                Developer = memberWithSkills ?? member,
                CompatibilityScore = compatibilityScore
            });
        }

        // Önerilen geliştiriciler: Bu projeye daveti olmayan, aktif, uyum >= %50
        var recommendedDevelopers = developersWithCompatibility
            .Where(d =>
            {
                var devId = ((DevTeamFinder.Models.Developer)d.GetType().GetProperty("Developer").GetValue(d)).Id;
                var hasInvitation = developerInvitationMap.ContainsKey(devId);
                var compatibility = (int)d.GetType().GetProperty("CompatibilityScore").GetValue(d);
                return !hasInvitation && compatibility >= 50;
            })
            .OrderByDescending(d => (int)d.GetType().GetProperty("CompatibilityScore").GetValue(d))
            .Take(5) // En fazla 5 öneri
            .ToList();

        ViewData["IsOwner"] = isOwner;
        ViewData["HasInvitation"] = hasInvitation;
        ViewData["CurrentInvitationStatus"] = currentInvitationStatus;
        ViewData["DevelopersWithInvitationStatus"] = developersWithInvitationStatus;
        ViewData["DevelopersWithCompatibility"] = developersWithCompatibility;
        ViewData["RecommendedDevelopers"] = recommendedDevelopers;
        ViewData["ProjectTeamMembers"] = projectTeamMembers;
        ViewData["TeamMembersWithCompatibility"] = teamMembersWithCompatibility;
        ViewData["Title"] = project.Baslik;

        // İletişim sekmesi için yetki kontrolü
        // Sadece proje sahibi veya projeye kabul edilmiş ekip üyeleri iletişim bilgilerini görebilir
        bool canViewContactInfo = false;
        List<dynamic>? teamContactInfo = null;

        if (isOwner)
        {
            // Proje sahibi iletişim bilgilerini görebilir
            canViewContactInfo = true;
        }
        else
        {
            // Kullanıcı bu projeye kabul edilmiş ekip üyesi mi kontrol et
            var userAcceptedInvitation = await _context.Invitations
                .FirstOrDefaultAsync(i => i.ProjectId == project.Id 
                    && i.DeveloperId == currentDeveloper.Id 
                    && i.Durum == "KabulEdildi");
            
            if (userAcceptedInvitation != null)
            {
                canViewContactInfo = true;
            }
        }

        // Eğer yetkili ise, iletişim bilgilerini hazırla
        if (canViewContactInfo)
        {
            // Proje sahibinin bilgileri
            var ownerInfo = new
            {
                Developer = project.Developer,
                Email = project.Developer.User.Email,
                Role = "Proje Sahibi"
            };

            // Ekip üyelerinin bilgileri
            var teamMembersInfo = projectTeamMembers.Select(member => new
            {
                Developer = member,
                Email = member.User.Email,
                Role = "Ekip Üyesi"
            }).ToList();

            // Hepsini birleştir
            teamContactInfo = new List<dynamic> { ownerInfo };
            teamContactInfo.AddRange(teamMembersInfo);
        }

        ViewData["CanViewContactInfo"] = canViewContactInfo;
        ViewData["TeamContactInfo"] = teamContactInfo;

        return View(project);
    }

    [HttpGet]
    public async Task<IActionResult> Public(string searchTerm, List<int> skillIds)
    {
        // Login kontrolü
        var redirectResult = RedirectToLoginIfNotAuthenticated();
        if (redirectResult != null)
        {
            return redirectResult;
        }

        // Tüm skill'leri view'a gönder (filtreleme için)
        var allSkills = await _context.Skills
            .OrderBy(s => s.Ad)
            .ToListAsync();

        // Base query: Aktif projeler
        var query = _context.Projects
            .Include(p => p.Developer)
            .Include(p => p.ProjectSkills)
                .ThenInclude(ps => ps.Skill)
            .Where(p => p.IsActive == true && p.Durum == "Aktif")
            .AsQueryable();

        // Arama filtresi: Başlık veya Açıklama
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var lowerSearchTerm = searchTerm.ToLower();
            query = query.Where(p => 
                p.Baslik.ToLower().Contains(lowerSearchTerm) || 
                p.Aciklama.ToLower().Contains(lowerSearchTerm));
        }

        // Skill filtresi: Seçilen skill'lerden EN AZ birine sahip projeler
        if (skillIds != null && skillIds.Any())
        {
            query = query.Where(p => p.ProjectSkills.Any(ps => skillIds.Contains(ps.SkillId)));
        }

        // Projeleri getir
        var projects = await query
            .OrderByDescending(p => p.Id)
            .ToListAsync();

        // View'a gönder
        ViewData["Title"] = "Proje Havuzu";
        ViewData["AllSkills"] = allSkills;
        ViewData["SearchTerm"] = searchTerm;
        ViewData["SelectedSkillIds"] = skillIds ?? new List<int>();
        
        return View(projects);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
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

        // Projeyi bul (sadece aktif projeler)
        var project = await _context.Projects
            .Where(p => p.Id == id && p.IsActive == true)
            .FirstOrDefaultAsync();

        if (project == null)
        {
            return NotFound();
        }

        // User → Developer'ı bul
        var currentDeveloper = await _context.Developers
            .FirstOrDefaultAsync(d => d.UserId == userId.Value);

        if (currentDeveloper == null)
        {
            return RedirectToAction("Login", "Account");
        }

        // Proje sahibi kontrolü - Sadece proje sahibi düzenleyebilir
        if (project.DeveloperId != currentDeveloper.Id)
        {
            TempData["ErrorMessage"] = "Bu projeyi düzenleme yetkiniz yok.";
            return RedirectToAction("Index");
        }

        ViewData["Title"] = "Proje Düzenle";
        return View(project);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, string baslik, string aciklama, string durum)
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

        // Basit validasyon
        if (string.IsNullOrWhiteSpace(baslik))
        {
            ModelState.AddModelError("baslik", "Proje başlığı zorunludur.");
            ViewData["Title"] = "Proje Düzenle";
            
            // Projeyi tekrar yükle
            var project = await _context.Projects
                .Where(p => p.Id == id && p.IsActive == true)
                .FirstOrDefaultAsync();
            
            if (project != null)
            {
                return View(project);
            }
            return NotFound();
        }

        try
        {
            // Projeyi bul
            var project = await _context.Projects
                .Where(p => p.Id == id && p.IsActive == true)
                .FirstOrDefaultAsync();

            if (project == null)
            {
                return NotFound();
            }

            // User → Developer'ı bul
            var currentDeveloper = await _context.Developers
                .FirstOrDefaultAsync(d => d.UserId == userId.Value);

            if (currentDeveloper == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Proje sahibi kontrolü - Sadece proje sahibi düzenleyebilir
            if (project.DeveloperId != currentDeveloper.Id)
            {
                TempData["ErrorMessage"] = "Bu projeyi düzenleme yetkiniz yok.";
                return RedirectToAction("Index");
            }

            // Sadece Baslik, Aciklama ve Durum güncellenir
            project.Baslik = baslik;
            project.Aciklama = aciklama ?? string.Empty;
            project.Durum = durum ?? "Aktif";

            await _context.SaveChangesAsync();

            _logger.LogInformation("Proje güncellendi: ProjectId={ProjectId}, Baslik={Baslik}", 
                id, baslik);

            TempData["SuccessMessage"] = "Proje başarıyla güncellendi.";
            return RedirectToAction("Details", new { id = project.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Proje güncelleme sırasında hata oluştu: ProjectId={ProjectId}", id);
            ModelState.AddModelError("", "Proje güncellenirken bir hata oluştu. Lütfen tekrar deneyin.");
            
            // Projeyi tekrar yükle
            var project = await _context.Projects
                .Where(p => p.Id == id && p.IsActive == true)
                .FirstOrDefaultAsync();
            
            if (project != null)
            {
                ViewData["Title"] = "Proje Düzenle";
                return View(project);
            }
            return NotFound();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
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
            // Projeyi bul (IsActive kontrolü yapmıyoruz, çünkü soft delete edilecek)
            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
            {
                return NotFound();
            }

            // User → Developer'ı bul
            var currentDeveloper = await _context.Developers
                .FirstOrDefaultAsync(d => d.UserId == userId.Value);

            if (currentDeveloper == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Proje sahibi kontrolü - Sadece proje sahibi silebilir
            if (project.DeveloperId != currentDeveloper.Id)
            {
                TempData["ErrorMessage"] = "Bu projeyi silme yetkiniz yok.";
                return RedirectToAction("Index");
            }

            // Soft Delete - DB'den silinmez, sadece IsActive = false yapılır
            project.IsActive = false;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Proje soft delete edildi: ProjectId={ProjectId}, Baslik={Baslik}", 
                id, project.Baslik);

            TempData["SuccessMessage"] = "Proje başarıyla silindi.";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Proje silme sırasında hata oluştu: ProjectId={ProjectId}", id);
            TempData["ErrorMessage"] = "Proje silinirken bir hata oluştu.";
            return RedirectToAction("Index");
        }
    }
}
