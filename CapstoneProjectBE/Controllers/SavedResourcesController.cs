using CapstoneProjectBE.DTOs.SavedResources;
using CapstoneProjectBE.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CapstoneProjectBE.Controllers;

[ApiController]
[Route("api/saved-resources")]
public class SavedResourcesController : ControllerBase
{
    private readonly ISavedResourceService _service;
    public SavedResourcesController(ISavedResourceService service) => _service = service;

    // TODO: Replace userID query parameter with authenticated user claims when auth is integrated.
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<SavedResourceDto>>> Get([FromQuery] Guid userID, CancellationToken ct)
    {
        if (userID == Guid.Empty) return BadRequest(new { message = "A valid userID is required." });
        return Ok(await _service.GetAllAsync(userID, ct));
    }

    [HttpPost("{resourceID:guid}")]
    public async Task<ActionResult<SavedResourceDto>> Save(Guid resourceID, [FromQuery] Guid userID, CancellationToken ct)
    {
        if (userID == Guid.Empty || resourceID == Guid.Empty)
            return BadRequest(new { message = "Valid userID and resourceID are required." });
        try { return Ok(await _service.SaveAsync(userID, resourceID, ct)); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }

    [HttpDelete("{resourceID:guid}")]
    public async Task<IActionResult> Delete(Guid resourceID, [FromQuery] Guid userID, CancellationToken ct)
    {
        if (userID == Guid.Empty || resourceID == Guid.Empty)
            return BadRequest(new { message = "Valid userID and resourceID are required." });
        return await _service.DeleteAsync(userID, resourceID, ct) ? NoContent() : NotFound();
    }
}
