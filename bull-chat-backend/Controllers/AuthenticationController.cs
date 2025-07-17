using bull_chat_backend.Models;
using bull_chat_backend.Services;
using bull_chat_backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace bull_chat_backend.Controllers
{
    [Route("api/[controller]/")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly ILogger<AuthenticationController> _logger;
        private readonly IUserRegistrationService _userRegistrationService;

        public AuthenticationController(ILogger<AuthenticationController> logger, IUserRegistrationService userRegistrationService)
        {
            _logger = logger;
            _userRegistrationService = userRegistrationService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterRequest request, CancellationToken token)
        {
            try
            {
                var user = await _userRegistrationService.RegisterAsync(request.Login, request.Password, token);
                if (Models.DBase.User.IsEmpty(user))
                    return BadRequest("Бычек пал");
                return Ok("Бычек прошел");

            }
            catch (InvalidDataException ex)
            {
                return UnprocessableEntity(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserRegisterRequest request, CancellationToken token)
        {
            var jwtToken = await _userRegistrationService.LoginAsync(request.Login, request.Password, token);

            if (string.IsNullOrEmpty(jwtToken))
                return BadRequest("Бычек пал");

            return Ok(new
            {
                Token = $"Bearer {jwtToken}"
            });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            return Ok("Бычек ушел");
        }

        [Authorize]
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok("Успешно!");
        }
    }
}
