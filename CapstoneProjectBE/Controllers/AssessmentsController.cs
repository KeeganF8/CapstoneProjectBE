using CapstoneProjectBE.DTOs.Assessments;
using CapstoneProjectBE.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CapstoneProjectBE.Controllers;

[ApiController]
[Route("api/assessments")]
public class AssessmentsController : ControllerBase
{
    private readonly IAssessmentService _service;
    public AssessmentsController(IAssessmentService service) => _service = service;

    [HttpPost]
    public async Task<ActionResult<AssessmentDto>> Create(CreateAssessmentDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var created = await _service.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.AssessmentID }, created);
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AssessmentDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var assessment = await _service.GetByIdAsync(id, cancellationToken);
        return assessment is null ? NotFound() : Ok(assessment);
    }

    [HttpGet("skill/{skillID:guid}")]
    public async Task<ActionResult<IReadOnlyList<AssessmentDto>>> GetBySkill(Guid skillID, CancellationToken cancellationToken)
    {
        var assessments = await _service.GetBySkillAsync(skillID, cancellationToken);
        return Ok(assessments);
    }

    [HttpPost("{id:guid}/submit")]
    public async Task<ActionResult<AssessmentResultDto>> Submit(Guid id, SubmitAssessmentDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.SubmitAsync(id, dto, cancellationToken);
            return result is null ? NotFound(new { message = "Assessment not found." }) : Ok(result);
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
    }
}
