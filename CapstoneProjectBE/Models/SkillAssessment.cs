namespace CapstoneProjectBE.Models;

public class SkillAssessment
{
    public Guid AssessmentID { get; set; }
    public Guid SkillID { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public Skill Skill { get; set; } = null!;
    public ICollection<AssessmentQuestion> Questions { get; set; } = new List<AssessmentQuestion>();
    public ICollection<AssessmentResponse> Attempts { get; set; } = new List<AssessmentResponse>();
}
