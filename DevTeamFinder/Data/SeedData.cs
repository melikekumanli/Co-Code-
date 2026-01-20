using Microsoft.EntityFrameworkCore;

namespace DevTeamFinder.Data;

public static class SeedData
{
    public static void EnsureSkills(AppDbContext context, ILogger logger)
    {
        // Migration'lar tabloyu oluşturmuş olmalı; yine de güvenli olması için try/catch üst seviyede.
        var desired = SkillCatalog.Skills;

        var existing = context.Skills
            .AsNoTracking()
            .Select(s => new { s.Id, s.Ad })
            .ToDictionary(x => x.Id, x => x.Ad);

        var toAdd = new List<Models.Skill>();
        var toUpdate = new List<Models.Skill>();

        foreach (var s in desired)
        {
            if (!existing.TryGetValue(s.Id, out var currentName))
            {
                toAdd.Add(new Models.Skill { Id = s.Id, Ad = s.Ad });
                continue;
            }

            if (!string.Equals(currentName, s.Ad, StringComparison.Ordinal))
            {
                toUpdate.Add(new Models.Skill { Id = s.Id, Ad = s.Ad });
            }
        }

        if (toAdd.Count == 0 && toUpdate.Count == 0)
        {
            logger.LogInformation("Skills seed sync: zaten güncel. Count={Count}", existing.Count);
            return;
        }

        if (toAdd.Count > 0)
        {
            context.Skills.AddRange(toAdd);
        }

        if (toUpdate.Count > 0)
        {
            context.Skills.UpdateRange(toUpdate);
        }

        context.SaveChanges();

        logger.LogInformation(
            "Skills seed sync tamamlandı. Added={Added}, Updated={Updated}, DesiredTotal={DesiredTotal}",
            toAdd.Count, toUpdate.Count, desired.Count);
    }
}


