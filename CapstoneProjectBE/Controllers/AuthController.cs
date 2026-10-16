using Microsoft.AspNetCore.Mvc;
using CapstoneProjectBE.DTOs.Auth;
using CapstoneProjectBE.Services.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;


namespace CapstoneProjectBE.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<RegisterResponse>> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
        {
            RegisterResponse? response = await _authService.RegisterAsync(request, cancellationToken);

            if (response is null)
            {
                return Conflict(new
                {
                    message = "Email already exists."
                });
            }

            return StatusCode(StatusCodes.Status201Created, response);
        }

        [HttpPost("verify-email")]
        public async Task<ActionResult<VerifyEmailResponse>> VerifyEmail([FromBody]  VerifyEmailRequest request, CancellationToken cancellationToken)
        {
            VerifyEmailResponse? response = await _authService.VerifyEmailAsync(request, cancellationToken);

            if(response is null)
            {
                return BadRequest(new
                {
                    message = "Invalid/expired verification code."
                });
            }

            return Ok(response);
        }

        [HttpPost("resend-verification")]
        public async Task<IActionResult> ResendVerification([FromBody] ResendVerificationRequest request, CancellationToken cancellationToken)
        {
            await _authService.ResendVerificationAsync(request, cancellationToken);

            return Ok(new
            {
                message = "Verification code has been sent."
            });
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
        {
            LoginResponse? response = await _authService.LoginAsync(request, cancellationToken);

            if(response is null)
            {
                return Unauthorized(new
                {
                    message = "Invalid email or password."
                });
            }
            return Ok(response);
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<AuthUserDto>> GetCurrentUser(CancellationToken cancellationToken)
        {
            string? userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if(!Guid.TryParse(userIdClaim, out Guid userId))
            {
                return Unauthorized();
            }

            AuthUserDto? user = await _authService.GetCurrentUserAsync(userId, cancellationToken);

            if(user is null)
            {
                return Unauthorized();
            }
            return Ok(user);
        }
    }
}
