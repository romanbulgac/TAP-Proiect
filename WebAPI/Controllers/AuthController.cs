using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Interfaces;
using BusinessLayer.DTOs;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization; // Required for [AllowAnonymous]

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [AllowAnonymous] // Ensure this endpoint is accessible without authentication
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authenticationService.RegisterAsync(registerRequestDto);

            if (!result.IsSuccess)
            {
                return BadRequest(new { message = result.ErrorMessage });
            }

            // Optionally, return the created user details or just a success message with token
            return Ok(new { token = result.Token, user = result.User });
        }

        [AllowAnonymous] // Ensure this endpoint is accessible without authentication
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authenticationService.LoginAsync(loginRequestDto);

            if (!result.IsSuccess)
            {
                return Unauthorized(new { message = result.ErrorMessage });
            }

            return Ok(new { token = result.Token, user = result.User });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequestDto tokenRequestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authenticationService.RefreshTokenAsync(tokenRequestDto);

            if (!result.IsSuccess)
            {
                return Unauthorized(new { message = result.ErrorMessage });
            }

            return Ok(new { token = result.Token, refreshToken = result.RefreshToken, user = result.User });
        }

        [Authorize]
        [HttpPost("revoke-token")]
        public async Task<IActionResult> RevokeToken([FromBody] string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
                return BadRequest(new { message = "Refresh token is required" });
            }

            var success = await _authenticationService.RevokeTokenAsync(refreshToken);

            if (!success)
            {
                return BadRequest(new { message = "Failed to revoke token. Invalid token or token already revoked." });
            }

            return Ok(new { message = "Token revoked successfully" });
        }
    }
}
