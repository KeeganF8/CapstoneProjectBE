using CapstoneProjectBE.DTOs.Dashboard;
using CapstoneProjectBE.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CapstoneProjectBE.Controllers;

[ApiController]
[Route("api/dashboard")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _service;
    public DashboardController(IDashboardService service) => _service = service;

    // TODO: Replace userID query parameter with authenticated user claims when auth is integrated.
    [HttpGet]
    public async Task<ActionResult<DashboardDto>> Get([FromQuery] Guid userID, CancellationToken ct)
    {
        if (userID == Guid.Empty) return BadRequest(new { message = "A valid userID is required." });
        var dashboard = await _service.GetAsync(userID, ct);
        return dashboard is null ? NotFound(new { message = "User not found." }) : Ok(dashboard);
    }
}
