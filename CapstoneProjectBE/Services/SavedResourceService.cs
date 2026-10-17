using CapstoneProjectBE.Data;
using CapstoneProjectBE.DTOs.SavedResources;
using CapstoneProjectBE.Models;
using CapstoneProjectBE.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CapstoneProjectBE.Services;

public class SavedResourceService : ISavedResourceService
{
    private readonly AppDbContext _db;
    public SavedResourceService(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<SavedResourceDto>> GetAllAsync(Guid userId, CancellationToken ct)
    {
        return await _db.SavedResources.AsNoTracking()
            .Where(s => s.UserID == userId)
            .Where(s => s.Resource.IsActive)
            .OrderByDescending(s => s.SavedAt)
            .Select(s => new SavedResourceDto
            {
                SavedResourceID = s.SavedResourceID,
                ResourceID = s.ResourceID,
                ResourceName = s.Resource.ResourceName,
                ResourceType = s.Resource.ResourceType,
                Url = s.Resource.Url,
                Provider = s.Resource.Provider,
                DifficultyLevel = s.Resource.DifficultyLevel,
                CostType = s.Resource.CostType,
                EstDuration = s.Resource.EstDuration,
                Description = s.Resource.Description,
                SavedAt = s.SavedAt
            }).ToListAsync(ct);
    }

    public async Task<SavedResourceDto?> SaveAsync(Guid userId, Guid resourceId, CancellationToken ct)
    {
        if (!await _db.Users.AnyAsync(u => u.UserID == userId, ct))
            throw new KeyNotFoundException("User not found.");
        var resource = await _db.LearningResources.AsNoTracking()
            .SingleOrDefaultAsync(r => r.ResourceID == resourceId && r.IsActive, ct);
        if (resource is null) throw new KeyNotFoundException("Active learning resource not found.");

        var saved = await _db.SavedResources.SingleOrDefaultAsync(
            s => s.UserID == userId && s.ResourceID == resourceId, ct);
        if (saved is null)
        {
            saved = new SavedResource
            {
                SavedResourceID = Guid.NewGuid(), UserID = userId,
                ResourceID = resourceId, SavedAt = DateTime.UtcNow
            };
            _db.SavedResources.Add(saved);
            await _db.SaveChangesAsync(ct);
        }
        return new SavedResourceDto
        {
            SavedResourceID = saved.SavedResourceID, ResourceID = resource.ResourceID,
            ResourceName = resource.ResourceName, ResourceType = resource.ResourceType,
            Url = resource.Url, Provider = resource.Provider, DifficultyLevel = resource.DifficultyLevel,
            CostType = resource.CostType, EstDuration = resource.EstDuration,
            Description = resource.Description, SavedAt = saved.SavedAt
        };
    }

    public async Task<bool> DeleteAsync(Guid userId, Guid resourceId, CancellationToken ct)
    {
        var saved = await _db.SavedResources.SingleOrDefaultAsync(
            s => s.UserID == userId && s.ResourceID == resourceId, ct);
        if (saved is null) return false;
        _db.SavedResources.Remove(saved);
        await _db.SaveChangesAsync(ct);
        return true;
    }
}
