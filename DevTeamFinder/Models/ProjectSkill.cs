namespace DevTeamFinder.Models;

public class ProjectSkill
{
    public int ProjectId { get; set; }
    public int SkillId { get; set; }

    // Navigation Properties
    public Project Project { get; set; }
    public Skill Skill { get; set; }
}
