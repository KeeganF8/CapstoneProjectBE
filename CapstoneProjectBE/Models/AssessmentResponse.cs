namespace CapstoneProjectBE.Models;

// One row represents one submitted attempt for one user and assessment.
public class AssessmentResponse
{
    public Guid ResponseID { get; set; }
    public Guid UserID { get; set; }
    public Guid AssessmentID { get; set; }
    public int AttemptNumber { get; set; }
    public decimal ScorePercentage { get; set; }
    public int CorrectAnswers { get; set; }
    public int TotalQuestions { get; set; }
    public string SubmittedAnswersJson { get; set; } = "{}";
    public DateTime SubmittedAt { get; set; }
    public User User { get; set; } = null!;
    public SkillAssessment Assessment { get; set; } = null!;
}
