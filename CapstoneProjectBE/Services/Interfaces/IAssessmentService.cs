using CapstoneProjectBE.DTOs.Assessments;

namespace CapstoneProjectBE.Services.Interfaces;

public interface IAssessmentService
{
    Task<AssessmentDto> CreateAsync(CreateAssessmentDto dto, CancellationToken cancellationToken);
    Task<AssessmentDto?> GetByIdAsync(Guid assessmentId, CancellationToken cancellationToken);
    Task<IReadOnlyList<AssessmentDto>> GetBySkillAsync(Guid skillId, CancellationToken cancellationToken);
    Task<AssessmentResultDto?> SubmitAsync(Guid assessmentId, SubmitAssessmentDto dto, CancellationToken cancellationToken);
}
