namespace CapstoneProjectBE.DTOs.Dashboard;

public class DashboardDto
{
    public Guid UserID { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? TargetCareer { get; set; }
    public int TotalSkills { get; set; }
    public int CvDetectedSkills { get; set; }
    public int KnowledgeVerifiedSkills { get; set; }
    public int TotalRoadmaps { get; set; }
    public decimal AverageRoadmapProgress { get; set; }
    public int SavedResources { get; set; }
}
