using CapstoneProjectBE.Services.Interfaces;
using CapstoneProjectBE.DTOs.Roadmaps;
using CapstoneProjectBE.DTOs.Resources;
using CapstoneProjectBE.Data;
using Microsoft.EntityFrameworkCore;


namespace CapstoneProjectBE.Services
{
    public class RoadmapService : IRoadmapService
    {
        //roadmaps, steps, and resource progress
        //completing resources updates progress, does not verify skill
        private readonly AppDbContext _context;

        public RoadmapService(AppDbContext context)
        {
            _context = context;
        }
        
        // GET /api/roadmaps/items/{id}/resources
        public async Task<List<RoadmapItemResourceDto>?> GetItemResourceAsync (Guid roadmapItemID)
        {

            //step not found = null
            bool itemExists = await _context.RoadmapItems
            .AnyAsync(item => item.RoadmapItemID == roadmapItemID);

            if (!itemExists)
            {
                return null;
            }

            return await _context.RoadmapItemResources
            .AsNoTracking()
            .Where(link => link.RoadmapItemID == roadmapItemID)
            .OrderBy(link => link.LearningResource.ResourceName)
            .Select(link => new RoadmapItemResourceDto
            {
                RoadmapItemResourceID = link.RoadmapItemResourceID,
                IsCompleted = link.IsCompleted,
                CompletedAt = link.CompletedAt,

                //match fields in LearningResourceDto
                Resource = new LearningResourceDto
                {
                    ResourceID = link.LearningResource.ResourceID,
                    SkillID = link.LearningResource.SkillID,
                    ResourceName = link.LearningResource.ResourceName,
                    ResourceType = link.LearningResource.ResourceType,
                    Url = link.LearningResource.Url,
                    Provider = link.LearningResource.Provider,
                    DifficultyLevel = link.LearningResource.DifficultyLevel,
                    CostType = link.LearningResource.CostType,
                    EstDuration = link.LearningResource.EstDuration,
                    Description = link.LearningResource.Description

                }
            })
            .ToListAsync();

             
        }

        //GET /api/roadmaps/items/{id}
        public async Task<RoadmapItemDto?> GetItemByIdAsync (Guid roadmapItemID)
        {
            return await _context.RoadmapItems
            .AsNoTracking()
            .Where(item => item.RoadmapItemID == roadmapItemID)
            .Select(item => new RoadmapItemDto
            {
                RoadmapItemID = item.RoadmapItemID,
                RoadmapID = item.RoadmapID,
                SkillID = item.SkillID,
                SkillName = item.Skill.SkillName,
                Title = item.Title,
                Description = item.Description,
                StepOrder = item.StepOrder,
                Priority = item.Priority,
                Status = item.Status,
                TargetDate = item.TargetDate,
                CompletedAt = item.CompletedAt,
                
            })
            .FirstOrDefaultAsync(); //null if not found
        }

        //GET /api/roadmaps/{id}
        public async Task<RoadmapDto?> GetRoadmapByIdAsync (Guid roadmapID)
        {
            return await _context.LearningRoadmaps
            .AsNoTracking()
            .Where(roadmap => roadmap.RoadmapID == roadmapID)
            .Select(roadmap => new RoadmapDto
            {
                RoadmapID = roadmap.RoadmapID,
                UserID = roadmap.UserID,
                CareerID = roadmap.CareerID,
                CareerName = roadmap.Career.CareerName,
                Title = roadmap.Title,
                ProgressPercentage = roadmap.ProgressPercentage,
                CreatedAt = roadmap.CreatedAt,
                UpdatedAt = roadmap.UpdatedAt,

                //steps in order
                Items = roadmap.Items
                .OrderBy(item => item.StepOrder)
                .Select(item => new RoadmapItemDto
                {
                    RoadmapItemID = item.RoadmapItemID,
                    RoadmapID = item.RoadmapID,
                    SkillID = item.SkillID,
                    SkillName = item.Skill.SkillName,
                    Title = item.Title,
                    Description = item.Description,
                    StepOrder = item.StepOrder,
                    Priority = item.Priority,
                    Status = item.Status,
                    TargetDate = item.TargetDate,
                    CompletedAt = item.CompletedAt
                })
                .ToList()
            })
            .FirstOrDefaultAsync();
        }

        //GET /api/roadmaps
        public async Task<List<RoadmapSummaryDto>> GetRoadmapForUserAsync(Guid userID)
        {
            return await _context.LearningRoadmaps
            .AsNoTracking()
            .Where(roadmap => roadmap.UserID == userID)
            .OrderByDescending(roadmap => roadmap.UpdatedAt)
            .Select(roadmap => new RoadmapSummaryDto
            {
                RoadmapID = roadmap.RoadmapID,
                CareerID = roadmap.CareerID,
                CareerName = roadmap.Career.CareerName,
                Title = roadmap.Title,
                ProgressPercentage = roadmap.ProgressPercentage,
                CreatedAt = roadmap.CreatedAt,
                UpdatedAt = roadmap.UpdatedAt
            })
            .ToListAsync();
            
        }

    }
}