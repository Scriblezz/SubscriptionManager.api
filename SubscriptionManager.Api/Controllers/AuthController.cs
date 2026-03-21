using Microsoft.AspNetCore.Mvc;
using SubscriptionManager.Api.Entities;
using SubscriptionManager.Api.Services;
using SubscriptionManager.Api.DTO.Users;

namespace SubscriptionManager.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register(
            [FromBody] UserRegistrationRequest request)
        {
            var result = await _authService.RegisterAsync(request);
            return Ok(result);
        }
    }
}