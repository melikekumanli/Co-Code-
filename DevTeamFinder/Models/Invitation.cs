namespace DevTeamFinder.Models;

public class Invitation
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public int DeveloperId { get; set; }
    public int SenderId { get; set; } // Daveti/İsteği gönderen kişinin DeveloperId'si
    public string Durum { get; set; } = "Beklemede";
    public string? Not { get; set; } // Davet notu (opsiyonel)

    // Navigation Properties
    public Project Project { get; set; }
    public Developer Developer { get; set; }
    public Developer Sender { get; set; } // Gönderen developer
}
