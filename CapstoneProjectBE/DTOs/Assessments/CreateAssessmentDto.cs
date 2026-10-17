using System.ComponentModel.DataAnnotations;

namespace CapstoneProjectBE.DTOs.Assessments;

public class CreateAssessmentDto
{
    [Required] public Guid SkillID { get; set; }
    [Required, StringLength(150, MinimumLength = 3)] public string Title { get; set; } = string.Empty;
    [StringLength(2000)] public string Description { get; set; } = string.Empty;
    [Required, MinLength(1)] public List<CreateAssessmentQuestionDto> Questions { get; set; } = new();
}

public class CreateAssessmentQuestionDto
{
    [Required, StringLength(1000, MinimumLength = 5)] public string QuestionText { get; set; } = string.Empty;
    [Required, MinLength(2)] public List<string> Options { get; set; } = new();
    [Required] public string CorrectAnswer { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
}
