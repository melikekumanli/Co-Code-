using Microsoft.EntityFrameworkCore;
using DevTeamFinder.Models;

namespace DevTeamFinder.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Developer> Developers { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Skill> Skills { get; set; }
    public DbSet<ProjectSkill> ProjectSkills { get; set; }
    public DbSet<DeveloperSkill> DeveloperSkills { get; set; }
    public DbSet<Invitation> Invitations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User ↔ Developer bire bir ilişki
        modelBuilder.Entity<User>()
            .HasOne(u => u.Developer)
            .WithOne(d => d.User)
            .HasForeignKey<Developer>(d => d.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Developer ↔ Project bir-çok ilişki
        modelBuilder.Entity<Developer>()
            .HasMany<Project>()
            .WithOne(p => p.Developer)
            .HasForeignKey(p => p.DeveloperId)
            .OnDelete(DeleteBehavior.Cascade);

        // Project ↔ Skill çok-çok ilişki (ProjectSkill join table)
        modelBuilder.Entity<ProjectSkill>()
            .HasKey(ps => new { ps.ProjectId, ps.SkillId });

        modelBuilder.Entity<ProjectSkill>()
            .HasOne(ps => ps.Project)
            .WithMany(p => p.ProjectSkills)
            .HasForeignKey(ps => ps.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ProjectSkill>()
            .HasOne(ps => ps.Skill)
            .WithMany(s => s.ProjectSkills)
            .HasForeignKey(ps => ps.SkillId)
            .OnDelete(DeleteBehavior.Cascade);

        // Developer ↔ Skill çok-çok ilişki (DeveloperSkill join table)
        modelBuilder.Entity<DeveloperSkill>()
            .HasKey(ds => new { ds.DeveloperId, ds.SkillId });

        modelBuilder.Entity<DeveloperSkill>()
            .HasOne(ds => ds.Developer)
            .WithMany(d => d.DeveloperSkills)
            .HasForeignKey(ds => ds.DeveloperId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<DeveloperSkill>()
            .HasOne(ds => ds.Skill)
            .WithMany(s => s.DeveloperSkills)
            .HasForeignKey(ds => ds.SkillId)
            .OnDelete(DeleteBehavior.Cascade);

        // Project ↔ Invitation bir-çok ilişki
        modelBuilder.Entity<Project>()
            .HasMany(p => p.Invitations)
            .WithOne(i => i.Project)
            .HasForeignKey(i => i.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        // Developer ↔ Invitation bir-çok ilişki (Developer - Davet edilen)
        modelBuilder.Entity<Invitation>()
            .HasOne(i => i.Developer)
            .WithMany(d => d.Invitations)
            .HasForeignKey(i => i.DeveloperId)
            .OnDelete(DeleteBehavior.Restrict); // Cascade yerine Restrict kullanıyoruz

        // Developer ↔ Invitation bir-çok ilişki (Sender - Gönderen)
        modelBuilder.Entity<Invitation>()
            .HasOne(i => i.Sender)
            .WithMany()
            .HasForeignKey(i => i.SenderId)
            .OnDelete(DeleteBehavior.Restrict); // Cascade yerine Restrict kullanıyoruz

        // Invitation için unique index: Aynı ProjectId ve DeveloperId kombinasyonu tekrar edemez
        modelBuilder.Entity<Invitation>()
            .HasIndex(i => new { i.ProjectId, i.DeveloperId })
            .IsUnique();

        // Seed Skills
        SeedSkills(modelBuilder);
    }

    private void SeedSkills(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Skill>().HasData(SkillCatalog.Skills);
    }
}
