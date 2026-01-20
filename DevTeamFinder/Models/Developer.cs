namespace DevTeamFinder.Models;

public class Developer
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string AdSoyad { get; set; } = string.Empty;
    public string Hakkinda { get; set; } = string.Empty;
    public string DeneyimSeviyesi { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    // Navigation Properties
    public User User { get; set; }
    public ICollection<Invitation> Invitations { get; set; } = new List<Invitation>();
    public ICollection<DeveloperSkill> DeveloperSkills { get; set; } = new List<DeveloperSkill>();
}
