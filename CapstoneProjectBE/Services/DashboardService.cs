using CapstoneProjectBE.Data;
using CapstoneProjectBE.DTOs.Dashboard;
using CapstoneProjectBE.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CapstoneProjectBE.Services;

public class DashboardService : IDashboardService
{
    private readonly AppDbContext _db;
    public DashboardService(AppDbContext db) => _db = db;

    public async Task<DashboardDto?> GetAsync(Guid userId, CancellationToken ct)
    {
        var user = await _db.Users.AsNoTracking().Include(u => u.UserProfile)
            .SingleOrDefaultAsync(u => u.UserID == userId, ct);
        if (user is null) return null;

        var skills = await _db.UserSkills.AsNoTracking().Where(s => s.UserID == userId)
            .Select(s => new { s.HasCvEvidence, s.IsKnowledgeVerified }).ToListAsync(ct);
        var roadmaps = await _db.LearningRoadmaps.AsNoTracking().Where(r => r.UserID == userId)
            .Select(r => r.ProgressPercentage).ToListAsync(ct);
        var savedCount = await _db.SavedResources.CountAsync(s => s.UserID == userId, ct);
        string? targetCareer = null;
        if (user.UserProfile?.TargetCareerID is Guid careerId)
            targetCareer = await _db.Careers.AsNoTracking().Where(c => c.CareerID == careerId)
                .Select(c => c.CareerName).SingleOrDefaultAsync(ct);

        return new DashboardDto
        {
            UserID = user.UserID, Email = user.Email,
            FirstName = user.UserProfile?.FName, LastName = user.UserProfile?.LName,
            TargetCareer = targetCareer,
            TotalSkills = skills.Count,
            CvDetectedSkills = skills.Count(s => s.HasCvEvidence),
            KnowledgeVerifiedSkills = skills.Count(s => s.IsKnowledgeVerified),
            TotalRoadmaps = roadmaps.Count,
            AverageRoadmapProgress = roadmaps.Count == 0 ? 0 : Math.Round(roadmaps.Average(), 2),
            SavedResources = savedCount
        };
    }
}
