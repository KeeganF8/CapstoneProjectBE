using System.Text.Json;
using CapstoneProjectBE.Data;
using CapstoneProjectBE.DTOs.Assessments;
using CapstoneProjectBE.Models;
using CapstoneProjectBE.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CapstoneProjectBE.Services;

public class AssessmentService : IAssessmentService
{
    private readonly AppDbContext _context;

    public AssessmentService(AppDbContext context) => _context = context;

    public async Task<AssessmentDto> CreateAsync(CreateAssessmentDto dto, CancellationToken cancellationToken)
    {
        var skillExists = await _context.Skills.AnyAsync(s => s.SkillID == dto.SkillID, cancellationToken);
        if (!skillExists) throw new KeyNotFoundException("The specified skill does not exist.");
        if (dto.Questions.Count == 0) throw new ArgumentException("An assessment must contain at least one question.");

        foreach (var question in dto.Questions)
        {
            if (question.Options.Count < 2 || question.Options.Any(string.IsNullOrWhiteSpace))
                throw new ArgumentException("Each question must have at least two non-empty options.");
            if (!question.Options.Contains(question.CorrectAnswer, StringComparer.Ordinal))
                throw new ArgumentException("Each correct answer must exactly match one of that question's options.");
        }

        var assessment = new SkillAssessment
        {
            AssessmentID = Guid.NewGuid(),
            SkillID = dto.SkillID,
            Title = dto.Title.Trim(),
            Description = dto.Description.Trim(),
            CreatedAt = DateTime.UtcNow,
            Questions = dto.Questions.Select((q, index) => new AssessmentQuestion
            {
                QuestionID = Guid.NewGuid(),
                QuestionText = q.QuestionText.Trim(),
                OptionsJson = JsonSerializer.Serialize(q.Options),
                CorrectAnswer = q.CorrectAnswer,
                DisplayOrder = q.DisplayOrder > 0 ? q.DisplayOrder : index + 1
            }).ToList()
        };

        _context.SkillAssessments.Add(assessment);
        await _context.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(assessment.AssessmentID, cancellationToken)
            ?? throw new InvalidOperationException("Created assessment could not be retrieved.");
    }

    public async Task<AssessmentDto?> GetByIdAsync(Guid assessmentId, CancellationToken cancellationToken)
    {
        var assessment = await _context.SkillAssessments
            .AsNoTracking()
            .Include(a => a.Skill)
            .Include(a => a.Questions.OrderBy(q => q.DisplayOrder))
            .SingleOrDefaultAsync(a => a.AssessmentID == assessmentId, cancellationToken);

        return assessment is null ? null : ToDto(assessment);
    }

    public async Task<IReadOnlyList<AssessmentDto>> GetBySkillAsync(Guid skillId, CancellationToken cancellationToken)
    {
        var assessments = await _context.SkillAssessments
            .AsNoTracking()
            .Where(a => a.SkillID == skillId)
            .Include(a => a.Skill)
            .Include(a => a.Questions)
            .OrderBy(a => a.Title)
            .ToListAsync(cancellationToken);

        return assessments.Select(ToDto).ToList();
    }

    public async Task<AssessmentResultDto?> SubmitAsync(Guid assessmentId, SubmitAssessmentDto dto, CancellationToken cancellationToken)
    {
        var assessment = await _context.SkillAssessments
            .AsNoTracking()
            .Include(a => a.Questions)
            .SingleOrDefaultAsync(a => a.AssessmentID == assessmentId, cancellationToken);
        if (assessment is null) return null;

        var userExists = await _context.Users.AnyAsync(u => u.UserID == dto.UserID, cancellationToken);
        if (!userExists) throw new KeyNotFoundException("The specified user does not exist.");
        if (assessment.Questions.Count == 0) throw new InvalidOperationException("This assessment has no questions.");

        var questionById = assessment.Questions.ToDictionary(q => q.QuestionID);
        if (dto.Answers.Count != questionById.Count || dto.Answers.Keys.Any(id => !questionById.ContainsKey(id)))
            throw new ArgumentException("Submit exactly one answer for every question in this assessment.");

        foreach (var answer in dto.Answers)
        {
            var options = JsonSerializer.Deserialize<List<string>>(questionById[answer.Key].OptionsJson) ?? new();
            if (!options.Contains(answer.Value, StringComparer.Ordinal))
                throw new ArgumentException($"The submitted answer for question {answer.Key} is not a valid option.");
        }

        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        // Serialize submissions for this user + assessment so two simultaneous requests cannot reuse an attempt number.
        await _context.Database.ExecuteSqlInterpolatedAsync(
            $"SELECT pg_advisory_xact_lock(hashtext({dto.UserID.ToString()}), hashtext({assessmentId.ToString()}))",
            cancellationToken);

        var previousAttempts = await _context.AssessmentResponses
            .Where(r => r.UserID == dto.UserID && r.AssessmentID == assessmentId)
            .Select(r => (int?)r.AttemptNumber)
            .MaxAsync(cancellationToken) ?? 0;

        var correct = assessment.Questions.Count(q =>
            string.Equals(dto.Answers[q.QuestionID], q.CorrectAnswer, StringComparison.Ordinal));
        var total = assessment.Questions.Count;
        var score = Math.Round((decimal)correct * 100m / total, 2, MidpointRounding.AwayFromZero);
        var response = new AssessmentResponse
        {
            ResponseID = Guid.NewGuid(),
            UserID = dto.UserID,
            AssessmentID = assessmentId,
            AttemptNumber = previousAttempts + 1,
            ScorePercentage = score,
            CorrectAnswers = correct,
            TotalQuestions = total,
            SubmittedAnswersJson = JsonSerializer.Serialize(dto.Answers.ToDictionary(kv => kv.Key.ToString(), kv => kv.Value)),
            SubmittedAt = DateTime.UtcNow
        };
        _context.AssessmentResponses.Add(response);

        var userSkill = await _context.UserSkills.SingleOrDefaultAsync(
            us => us.UserID == dto.UserID && us.SkillID == assessment.SkillID, cancellationToken);
        if (userSkill is null)
        {
            userSkill = new UserSkill
            {
                UserSkillID = Guid.NewGuid(),
                UserID = dto.UserID,
                SkillID = assessment.SkillID,
                HasCvEvidence = false,
                IsKnowledgeVerified = false,
                UpdatedAt = DateTime.UtcNow
            };
            _context.UserSkills.Add(userSkill);
        }

        // Only a perfect score can turn verification on. A failed attempt never clears CV evidence
        // and never changes a previously achieved knowledge-verification flag.
        if (score == 100m)
        {
            userSkill.IsKnowledgeVerified = true;
            userSkill.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return new AssessmentResultDto
        {
            ResponseID = response.ResponseID,
            AssessmentID = response.AssessmentID,
            AttemptNumber = response.AttemptNumber,
            ScorePercentage = response.ScorePercentage,
            CorrectAnswers = response.CorrectAnswers,
            TotalQuestions = response.TotalQuestions,
            IsKnowledgeVerified = userSkill.IsKnowledgeVerified,
            SubmittedAt = response.SubmittedAt
        };
    }

    private static AssessmentDto ToDto(SkillAssessment assessment) => new()
    {
        AssessmentID = assessment.AssessmentID,
        SkillID = assessment.SkillID,
        SkillName = assessment.Skill.SkillName,
        Title = assessment.Title,
        Description = assessment.Description,
        Questions = assessment.Questions.OrderBy(q => q.DisplayOrder).Select(q => new AssessmentQuestionDto
        {
            QuestionID = q.QuestionID,
            QuestionText = q.QuestionText,
            Options = JsonSerializer.Deserialize<List<string>>(q.OptionsJson) ?? new List<string>(),
            DisplayOrder = q.DisplayOrder
        }).ToList()
    };
}
