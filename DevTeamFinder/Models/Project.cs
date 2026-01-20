namespace DevTeamFinder.Models;

public class Project
{
    public int Id { get; set; }
    public string Baslik { get; set; } = string.Empty;
    public string Aciklama { get; set; } = string.Empty;
    public string Durum { get; set; } = string.Empty;
    public int DeveloperId { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation Properties
    public Developer Developer { get; set; }
    public ICollection<ProjectSkill> ProjectSkills { get; set; } = new List<ProjectSkill>();
    public ICollection<Invitation> Invitations { get; set; } = new List<Invitation>();
}
