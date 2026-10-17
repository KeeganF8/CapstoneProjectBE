namespace CapstoneProjectBE.DTOs.Assessments;

public class AssessmentDto
{
    public Guid AssessmentID { get; set; }
    public Guid SkillID { get; set; }
    public string SkillName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public IReadOnlyList<AssessmentQuestionDto> Questions { get; set; } = Array.Empty<AssessmentQuestionDto>();
}

public class AssessmentQuestionDto
{
    public Guid QuestionID { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public IReadOnlyList<string> Options { get; set; } = Array.Empty<string>();
    public int DisplayOrder { get; set; }
}
