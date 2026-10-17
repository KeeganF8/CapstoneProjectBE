namespace CapstoneProjectBE.Models;

public class AssessmentQuestion
{
    public Guid QuestionID { get; set; }
    public Guid AssessmentID { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    // JSON array of options, e.g. ["Option A", "Option B"]. Never return CorrectAnswer before submission.
    public string OptionsJson { get; set; } = "[]";
    public string CorrectAnswer { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public SkillAssessment Assessment { get; set; } = null!;
}
