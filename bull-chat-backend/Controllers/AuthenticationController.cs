using bull_chat_backend.Models;
using bull_chat_backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace bull_chat_backend.Controllers
{
    [Route("api/[controller]/")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly ILogger<AuthenticationController> _logger;
        private readonly UserRegistrationService _userService;

        public AuthenticationController(ILogger<AuthenticationController> logger, UserRegistrationService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterRequest request, CancellationToken token)
        {
            try
            {
                var user = await _userService.RegisterAsync(request.Login, request.Password, token);
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
            var jwtToken = await _userService.LoginAsync(request.Login, request.Password, token);

            if (string.IsNullOrEmpty(jwtToken))
                return BadRequest("Бычек пал");

            HttpContext.Response.Cookies.Append("JWT", jwtToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddHours(2)
            });

            return Ok(new { Token = jwtToken });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            HttpContext.Response.Cookies.Delete("JWT");
            return Ok("Бычек ушел");
        }
    }
}
