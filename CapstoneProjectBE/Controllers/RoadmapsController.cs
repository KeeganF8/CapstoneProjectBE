using CapstoneProjectBE.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using CapstoneProjectBE.DTOs.Roadmaps;

namespace CapstoneProjectBE.Controllers
{
    [ApiController]
    [Route("api/roadmaps")]
    public class RoadmapsController : ControllerBase
    {
        private readonly IRoadmapService _roadmapService;

        //TODO: replace with logged-in user's ID 
        private static readonly Guid TempUserID = Guid.Parse("00000000-0000-0000-0000-000000000001");


        public RoadmapsController(IRoadmapService roadmapService)
        {
            _roadmapService = roadmapService;
        }

        //GET /api/roadmaps/items/{id}/resources
        [HttpGet("items/{id}/resources")]
        public async Task<ActionResult<List<RoadmapItemResourceDto>>> GetItemResources(Guid id)
        {
            var resources = await _roadmapService.GetItemResourceAsync(id);

            if (resources == null)
            {
                return NotFound();
            }
            return Ok(resources);
        }

        //GET /api/roadmaps/items/{id}
        [HttpGet("items/{id}")]
        public async Task<ActionResult<RoadmapItemDto>> GetItem(Guid id)
        {
            var item = await _roadmapService.GetItemByIdAsync(id);

            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }

        //GET /api/roadmaps/{id}
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<RoadmapDto>> GetRoadmap(Guid id)
        {
            var roadmap = await _roadmapService.GetRoadmapByIdAsync(id);

            if (roadmap == null)
            {
                return NotFound();
            }

            return Ok(roadmap);
        }

        //GET /api/roadmaps
        [HttpGet]
        public async Task<ActionResult<List<RoadmapSummaryDto>>> GetRoadmaps()
        {
            var roadmaps = await _roadmapService.GetRoadmapForUserAsync(TempUserID);
            return Ok(roadmaps);
        }

        //GET /api/roadmaps/current
        [HttpGet("current")]
        public async Task<ActionResult<RoadmapDto>> GetCurrentRoadmap()
        {
            var roadmap = await _roadmapService.GetCurrentRoadmapAsync(TempUserID);

            if (roadmap == null)
            {
                return NotFound();
            }
            return Ok(roadmap);
        }

        //POST /api/roadmaps/items/{itemID}/reosurces/{resourceID}/complete
        [HttpPost("items/{itemID}/resources/{resourceID}/complete")]
        public async Task<ActionResult<CompleteResourceResultDto>> CompleteResource (Guid itemID, Guid resourceID)
        {
            var result = await _roadmapService.CompleteResourceAsync(itemID, resourceID);

            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        //POST /api/roadmaps/generate
        [HttpPost("generate")]
        public async Task<ActionResult<RoadmapDto>> GenerateRoadmap(GenerateRoadmapDto request)
        {
            var roadmap = await _roadmapService.GenerateRoadmapAsync(TempUserID, request.CareerID);

            if (roadmap == null)
            {
                return NotFound();
            }

            return CreatedAtAction(nameof(GenerateRoadmap), new { id = roadmap.RoadmapID}, roadmap);
        }
    }
}