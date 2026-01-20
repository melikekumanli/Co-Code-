using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DevTeamFinder.Data;
using DevTeamFinder.Models;

namespace DevTeamFinder.Controllers;

public class InvitationsController : BaseController
{
    private readonly AppDbContext _context;
    private readonly ILogger<InvitationsController> _logger;

    public InvitationsController(AppDbContext context, ILogger<InvitationsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([FromForm] int projectId, [FromForm] int developerId, [FromForm] string? not)
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

        // developerId parametresi kontrolü
        if (developerId <= 0)
        {
            _logger.LogWarning("Geçersiz developerId: {DeveloperId}, ProjectId: {ProjectId}, UserId: {UserId}", 
                developerId, projectId, userId);
            TempData["ErrorMessage"] = "Geçersiz geliştirici seçimi.";
            return RedirectToAction("Details", "Projects", new { id = projectId });
        }

        try
        {
            // Projeyi bul (sadece aktif projeler)
            var project = await _context.Projects
                .Include(p => p.Developer)
                .Where(p => p.Id == projectId && p.IsActive == true)
                .FirstOrDefaultAsync();

            if (project == null)
            {
                return NotFound();
            }

            // Proje sahibi kontrolü
            var currentDeveloper = await _context.Developers
                .FirstOrDefaultAsync(d => d.UserId == userId.Value);

            if (currentDeveloper == null || project.DeveloperId != currentDeveloper.Id)
            {
                _logger.LogWarning("Yetkisiz davet oluşturma girişimi: ProjectId={ProjectId}, UserId={UserId}, CurrentDeveloperId={CurrentDeveloperId}, ProjectOwnerId={ProjectOwnerId}", 
                    projectId, userId, currentDeveloper?.Id, project.DeveloperId);
                TempData["ErrorMessage"] = "Bu projeye davet gönderme yetkiniz yok.";
                return RedirectToAction("Details", "Projects", new { id = projectId });
            }

            // Developer'ı bul (sadece aktif geliştiriciler)
            var developer = await _context.Developers
                .Where(d => d.Id == developerId && d.IsActive == true)
                .FirstOrDefaultAsync();

            if (developer == null)
            {
                _logger.LogWarning("Developer bulunamadı: DeveloperId={DeveloperId}, ProjectId={ProjectId}", 
                    developerId, projectId);
                TempData["ErrorMessage"] = "Seçilen geliştirici bulunamadı veya aktif değil.";
                return RedirectToAction("Details", "Projects", new { id = projectId });
            }

            // Proje sahibi kendine davet gönderemez
            if (developerId == currentDeveloper.Id)
            {
                TempData["ErrorMessage"] = "Kendi projenize kendinize davet gönderemezsiniz.";
                return RedirectToAction("Details", "Projects", new { id = projectId });
            }

            // Log: Davet oluşturulmadan önce parametreleri logla
            _logger.LogInformation("Davet oluşturuluyor: ProjectId={ProjectId}, DavetEdilenDeveloperId={DeveloperId}, ProjeSahibiId={CurrentDeveloperId}, UserId={UserId}", 
                projectId, developerId, currentDeveloper.Id, userId);

            // Duplicate kontrolü: Aynı ProjectId ve DeveloperId ile daha önce davet var mı?
            var existingInvitation = await _context.Invitations
                .FirstOrDefaultAsync(i => i.ProjectId == projectId && i.DeveloperId == developerId);

            if (existingInvitation != null)
            {
                // Duruma göre hata mesajı
                string errorMessage = existingInvitation.Durum switch
                {
                    "Beklemede" => "Bu geliştiriciye daha önce davet gönderilmiş. Davet hala bekleniyor.",
                    "KabulEdildi" => "Bu geliştirici zaten projeye katılmış.",
                    "Reddedildi" => "Bu geliştirici daha önce daveti reddetmiş.",
                    _ => "Bu geliştiriciye daha önce davet gönderilmiş."
                };

                TempData["ErrorMessage"] = errorMessage;
                return RedirectToAction("Details", "Projects", new { id = projectId });
            }

            // Davet oluştur (Proje sahibi developer'a davet gönderiyor)
            var invitation = new Invitation
            {
                ProjectId = projectId,
                DeveloperId = developerId, // Davet edilen developer
                SenderId = currentDeveloper.Id, // Daveti gönderen (proje sahibi)
                Durum = "Beklemede",
                Not = not // Davet notu
            };

            _context.Invitations.Add(invitation);
            await _context.SaveChangesAsync();

            // Oluşturulan daveti doğrula
            var savedInvitation = await _context.Invitations
                .Include(i => i.Developer)
                .FirstOrDefaultAsync(i => i.Id == invitation.Id);

            if (savedInvitation == null || savedInvitation.DeveloperId != developerId)
            {
                _logger.LogError("Davet kaydedilirken hata oluştu: BeklenenDeveloperId={DeveloperId}, KaydedilenDeveloperId={SavedDeveloperId}", 
                    developerId, savedInvitation?.DeveloperId);
                TempData["ErrorMessage"] = "Davet kaydedilirken bir hata oluştu.";
                return RedirectToAction("Details", "Projects", new { id = projectId });
            }

            _logger.LogInformation("Davet başarıyla gönderildi: InvitationId={InvitationId}, ProjectId={ProjectId}, DavetEdilenDeveloperId={DeveloperId}, DavetEdilenDeveloperAdi={DeveloperAdi}, ProjeSahibiId={CurrentDeveloperId}", 
                invitation.Id, projectId, developerId, developer.AdSoyad, currentDeveloper.Id);

            TempData["SuccessMessage"] = "Davet başarıyla gönderildi.";
            return RedirectToAction("Details", "Projects", new { id = projectId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Davet oluşturma sırasında hata oluştu: ProjectId={ProjectId}, DeveloperId={DeveloperId}", 
                projectId, developerId);
            TempData["ErrorMessage"] = "Davet gönderilirken bir hata oluştu.";
            return RedirectToAction("Details", "Projects", new { id = projectId });
        }
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

        // User → Developer'ı bul
        var developer = await _context.Developers
            .FirstOrDefaultAsync(d => d.UserId == userId.Value);

        if (developer == null)
        {
            return RedirectToAction("Login", "Account");
        }

        // Üç tip davet/istek var:
        // 1. Bana gönderilen davetler: DeveloperId == benim VE SenderId != benim (proje sahibi bana davet göndermiş)
        // 2. Benim gönderdiğim katılma istekleri: DeveloperId == benim VE SenderId == benim (ben projeye katılmak istiyorum)
        // 3. Projelerime gelen katılma istekleri: Project.DeveloperId == benim VE SenderId != benim (başkası projeme katılmak istiyor)
        var myInvitations = await _context.Invitations
            .Include(i => i.Project)
                .ThenInclude(p => p.Developer)
            .Include(i => i.Developer)
            .Include(i => i.Sender)
            .Include(i => i.Project)
                .ThenInclude(p => p.ProjectSkills)
                    .ThenInclude(ps => ps.Skill)
            .Where(i => (i.DeveloperId == developer.Id || i.Project.DeveloperId == developer.Id) 
                     && i.Project.IsActive == true)
            .OrderByDescending(i => i.Id)
            .ToListAsync();

        // Bana gönderilen davetler (proje sahibi tarafından gönderilen)
        // DeveloperId == benim VE SenderId != benim
        var receivedInvitations = myInvitations
            .Where(i => i.DeveloperId == developer.Id && i.SenderId != developer.Id)
            .ToList();

        // Benim gönderdiğim katılma istekleri
        // DeveloperId == benim VE SenderId == benim
        var sentRequests = myInvitations
            .Where(i => i.DeveloperId == developer.Id && i.SenderId == developer.Id)
            .ToList();

        // Projelerime gelen katılma istekleri (developer'ların gönderdiği)
        // Project.DeveloperId == benim VE SenderId != benim VE DeveloperId != benim
        var projectRequests = myInvitations
            .Where(i => i.Project.DeveloperId == developer.Id && i.SenderId != developer.Id && i.DeveloperId != developer.Id)
            .ToList();

        // Gruplara ayır - Bana gönderilen davetler
        var beklemede = receivedInvitations.Where(i => i.Durum == "Beklemede").ToList();
        var kabulEdildi = receivedInvitations.Where(i => i.Durum == "KabulEdildi").ToList();
        var reddedildi = receivedInvitations.Where(i => i.Durum == "Reddedildi").ToList();

        // Benim gönderdiğim katılma istekleri - Gruplara ayır
        var sentRequestsBeklemede = sentRequests.Where(i => i.Durum == "Beklemede").ToList();
        var sentRequestsKabulEdildi = sentRequests.Where(i => i.Durum == "KabulEdildi").ToList();
        var sentRequestsReddedildi = sentRequests.Where(i => i.Durum == "Reddedildi").ToList();

        // Projelerime gelen katılma istekleri - Gruplara ayır
        var projectRequestsBeklemede = projectRequests.Where(i => i.Durum == "Beklemede").ToList();
        var projectRequestsKabulEdildi = projectRequests.Where(i => i.Durum == "KabulEdildi").ToList();
        var projectRequestsReddedildi = projectRequests.Where(i => i.Durum == "Reddedildi").ToList();

        ViewData["Beklemede"] = beklemede;
        ViewData["KabulEdildi"] = kabulEdildi;
        ViewData["Reddedildi"] = reddedildi;
        ViewData["SentRequestsBeklemede"] = sentRequestsBeklemede;
        ViewData["SentRequestsKabulEdildi"] = sentRequestsKabulEdildi;
        ViewData["SentRequestsReddedildi"] = sentRequestsReddedildi;
        ViewData["ProjectRequestsBeklemede"] = projectRequestsBeklemede;
        ViewData["ProjectRequestsKabulEdildi"] = projectRequestsKabulEdildi;
        ViewData["ProjectRequestsReddedildi"] = projectRequestsReddedildi;
        ViewData["Title"] = "Davetlerim";

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Accept(int id)
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
            // Daveti bul
            var invitation = await _context.Invitations
                .Include(i => i.Developer)
                .Include(i => i.Project)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (invitation == null)
            {
                return NotFound();
            }

            // Login olan developer'ı bul
            var currentDeveloper = await _context.Developers
                .FirstOrDefaultAsync(d => d.UserId == userId.Value);

            if (currentDeveloper == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // İki senaryo:
            // 1. Davet edilen kişi (DeveloperId) daveti kabul ediyor
            // 2. Proje sahibi (ProjectOwnerId) katılma isteğini kabul ediyor
            bool isDeveloperAccepting = invitation.DeveloperId == currentDeveloper.Id;
            bool isProjectOwnerAccepting = invitation.Project.DeveloperId == currentDeveloper.Id;

            if (!isDeveloperAccepting && !isProjectOwnerAccepting)
            {
                _logger.LogWarning("Yetkisiz davet kabul etme girişimi: InvitationId={InvitationId}, UserId={UserId}", 
                    id, userId);
                TempData["ErrorMessage"] = "Bu daveti kabul etme yetkiniz yok.";
                return RedirectToAction("Index");
            }

            // Durumu güncelle
            invitation.Durum = "KabulEdildi";
            await _context.SaveChangesAsync();

            _logger.LogInformation("Davet kabul edildi: InvitationId={InvitationId}, KabulEden={AcceptedBy}", 
                id, isDeveloperAccepting ? "Developer" : "ProjectOwner");

            TempData["SuccessMessage"] = "Davet kabul edildi.";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Davet kabul etme sırasında hata oluştu: InvitationId={InvitationId}", id);
            TempData["ErrorMessage"] = "Davet kabul edilirken bir hata oluştu.";
            return RedirectToAction("Index");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(int id)
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
            // Daveti bul
            var invitation = await _context.Invitations
                .Include(i => i.Developer)
                .Include(i => i.Project)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (invitation == null)
            {
                return NotFound();
            }

            // Login olan developer'ı bul
            var currentDeveloper = await _context.Developers
                .FirstOrDefaultAsync(d => d.UserId == userId.Value);

            if (currentDeveloper == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // İki senaryo:
            // 1. Davet edilen kişi (DeveloperId) daveti reddediyor
            // 2. Proje sahibi (ProjectOwnerId) katılma isteğini reddediyor
            bool isDeveloperRejecting = invitation.DeveloperId == currentDeveloper.Id;
            bool isProjectOwnerRejecting = invitation.Project.DeveloperId == currentDeveloper.Id;

            if (!isDeveloperRejecting && !isProjectOwnerRejecting)
            {
                _logger.LogWarning("Yetkisiz davet reddetme girişimi: InvitationId={InvitationId}, UserId={UserId}", 
                    id, userId);
                TempData["ErrorMessage"] = "Bu daveti reddetme yetkiniz yok.";
                return RedirectToAction("Index");
            }

            // Durumu güncelle
            invitation.Durum = "Reddedildi";
            await _context.SaveChangesAsync();

            _logger.LogInformation("Davet reddedildi: InvitationId={InvitationId}, ReddEden={RejectedBy}", 
                id, isDeveloperRejecting ? "Developer" : "ProjectOwner");

            TempData["SuccessMessage"] = "Davet reddedildi.";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Davet reddetme sırasında hata oluştu: InvitationId={InvitationId}", id);
            TempData["ErrorMessage"] = "Davet reddedilirken bir hata oluştu.";
            return RedirectToAction("Index");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RequestToJoin(int projectId, string? not)
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
            // Projeyi bul (sadece aktif projeler)
            var project = await _context.Projects
                .Include(p => p.Developer)
                .Where(p => p.Id == projectId && p.IsActive == true && p.Durum == "Aktif")
                .FirstOrDefaultAsync();

            if (project == null)
            {
                return NotFound();
            }

            // Giriş yapan developer'ı bul
            var currentDeveloper = await _context.Developers
                .Where(d => d.UserId == userId.Value && d.IsActive == true)
                .FirstOrDefaultAsync();

            if (currentDeveloper == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Proje sahibi olamaz (kendi projesine katılamaz)
            if (project.DeveloperId == currentDeveloper.Id)
            {
                TempData["ErrorMessage"] = "Kendi projenize katılamazsınız.";
                return RedirectToAction("Details", "Projects", new { id = projectId });
            }

            // Duplicate kontrolü: Aynı ProjectId ve DeveloperId ile daha önce davet var mı?
            var existingInvitation = await _context.Invitations
                .FirstOrDefaultAsync(i => i.ProjectId == projectId && i.DeveloperId == currentDeveloper.Id);

            if (existingInvitation != null)
            {
                // Duruma göre hata mesajı
                string errorMessage = existingInvitation.Durum switch
                {
                    "Beklemede" => "Bu projeye daha önce katılım isteği gönderdiniz. Davet hala bekleniyor.",
                    "KabulEdildi" => "Bu projeye zaten katıldınız.",
                    "Reddedildi" => "Bu projeye daha önce katılım isteğiniz reddedilmiş.",
                    _ => "Bu projeye daha önce katılım isteği gönderdiniz."
                };

                TempData["ErrorMessage"] = errorMessage;
                return RedirectToAction("Details", "Projects", new { id = projectId });
            }

            // Katılma isteği oluştur (Developer proje sahibine istek gönderiyor)
            var invitation = new Invitation
            {
                ProjectId = projectId,
                DeveloperId = currentDeveloper.Id, // İstek gönderen developer (kendisi)
                SenderId = currentDeveloper.Id, // İsteği gönderen (kendisi)
                Durum = "Beklemede",
                Not = not // Katılma isteği notu
            };

            _context.Invitations.Add(invitation);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Katılım isteği gönderildi: ProjectId={ProjectId}, DeveloperId={DeveloperId}", 
                projectId, currentDeveloper.Id);

            TempData["SuccessMessage"] = "Katılım isteğiniz proje sahibine gönderildi. Onay bekliyor.";
            return RedirectToAction("Details", "Projects", new { id = projectId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Katılım isteği oluşturma sırasında hata oluştu: ProjectId={ProjectId}", 
                projectId);
            TempData["ErrorMessage"] = "Katılım isteği gönderilirken bir hata oluştu.";
            return RedirectToAction("Details", "Projects", new { id = projectId });
        }
    }
}
