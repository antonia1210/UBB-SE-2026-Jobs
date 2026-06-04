namespace UBB_SE_2026_Jobs.Library.Domain;

public class Company
{
    public int CompanyId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string? AboutUs { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string? ProfilePicturePath { get => ProfilePictureUrl; set => ProfilePictureUrl = value; }
    public string LogoUrl { get; set; } = string.Empty;
    public string? CompanyLogoPath { get => LogoUrl; set => LogoUrl = value; }
    public string? LogoText { get; set; }
    public string? Location { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public int PostedJobsCount { get; set; }
    public int CollaboratorsCount { get; set; }

    public Game? Game { get; set; }


    public string? BuddyName { get; set; }
    public int? AvatarId { get; set; }
    public string? FinalQuote { get; set; }
    public string? BuddyDescription { get; set; }

    public string? Scen1Text { get; set; }
    public string? Scen1Answer1 { get; set; }
    public string? Scen1Answer2 { get; set; }
    public string? Scen1Answer3 { get; set; }
    public string? Scen1Reaction1 { get; set; }
    public string? Scen1Reaction2 { get; set; }
    public string? Scen1Reaction3 { get; set; }

    public string? Scen2Text { get; set; }
    public string? Scen2Answer1 { get; set; }
    public string? Scen2Answer2 { get; set; }
    public string? Scen2Answer3 { get; set; }
    public string? Scen2Reaction1 { get; set; }
    public string? Scen2Reaction2 { get; set; }
    public string? Scen2Reaction3 { get; set; }

    public List<Job> Jobs { get; set; } = new();
    public List<UBB_SE_2026_Jobs.Library.Domain.Event> Events { get; set; } = new();
    public List<UBB_SE_2026_Jobs.Library.Domain.Collaborator> Collaborators { get; set; } = new();
}
