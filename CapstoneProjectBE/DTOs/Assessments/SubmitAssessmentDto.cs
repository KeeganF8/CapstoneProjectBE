using System.ComponentModel.DataAnnotations;

namespace CapstoneProjectBE.DTOs.Assessments;

public class SubmitAssessmentDto
{
    // TEMPORARY: replace with the authenticated user's ID from claims when authentication is merged.
    [Required] public Guid UserID { get; set; }
    [Required, MinLength(1)] public Dictionary<Guid, string> Answers { get; set; } = new();
}

public class AssessmentResultDto
{
    public Guid ResponseID { get; set; }
    public Guid AssessmentID { get; set; }
    public int AttemptNumber { get; set; }
    public decimal ScorePercentage { get; set; }
    public int CorrectAnswers { get; set; }
    public int TotalQuestions { get; set; }
    public bool IsKnowledgeVerified { get; set; }
    public DateTime SubmittedAt { get; set; }
}
