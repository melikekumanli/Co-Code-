namespace DevTeamFinder.Models;

public class Skill
{
    public int Id { get; set; }
    public string Ad { get; set; } = string.Empty;

    // Navigation Properties
    public ICollection<ProjectSkill> ProjectSkills { get; set; } = new List<ProjectSkill>();
    public ICollection<DeveloperSkill> DeveloperSkills { get; set; } = new List<DeveloperSkill>();
}
