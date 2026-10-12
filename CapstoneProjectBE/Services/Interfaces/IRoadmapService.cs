using CapstoneProjectBE.DTOs.Roadmaps;

namespace CapstoneProjectBE.Services.Interfaces
{
    public interface IRoadmapService
    {

        //resources for a roadmap step
        //null if the step does not exist
        Task<List<RoadmapItemResourceDto>?> GetItemResourceAsync(Guid roadmapItemID);

        //one step by its roadmap_item_id
        //null if not found
        Task<RoadmapItemDto?> GetItemByIdAsync (Guid roadmapItemID);

        //full roadmap by its roadmap_id, including all its steps
        //null if not found
        Task<RoadmapDto?> GetRoadmapByIdAsync (Guid roadmapID);

        //all roadmaps for a user, newest first
        Task<List<RoadmapSummaryDto>> GetRoadmapForUserAsync (Guid userID);

    }
}