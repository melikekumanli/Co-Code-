namespace DevTeamFinder.Models;

public class DeveloperSkill
{
    public int DeveloperId { get; set; }
    public int SkillId { get; set; }

    // Navigation Properties
    public Developer Developer { get; set; }
    public Skill Skill { get; set; }
}
