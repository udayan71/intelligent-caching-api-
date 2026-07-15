using Application.Constants;
using Application.DTOs.Auth;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace intelligent_caching_api.Controllers
{
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IAuthService authService,
            ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost(ApiRoutes.Auth.Register)]
        public async Task<IActionResult> Register(
            [FromBody] RegisterRequestDto request)
        {
            try
            {
                var result =
                    await _authService.RegisterAsync(
                        request);

                return Ok(result);
            }
            catch (ApplicationException ex)
            {
                _logger.LogWarning(
                    ex,
                    "Registration failed");

                return BadRequest(new
                {
                    Message = ex.Message
                });
            }
        }

        [HttpPost(ApiRoutes.Auth.Login)]
        public async Task<IActionResult> Login(
            [FromBody] LoginRequestDto request)
        {
            try
            {
                var result =
                    await _authService.LoginAsync(
                        request);

                return Ok(result);
            }
            catch (ApplicationException ex)
            {
                _logger.LogWarning(
                    ex,
                    "Login failed");

                return Unauthorized(new
                {
                    Message = ex.Message
                });
            }
        }
    }
}
