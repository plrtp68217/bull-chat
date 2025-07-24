using bull_chat_backend.Extensions;
using bull_chat_backend.ModelBindings.Attributes;
using bull_chat_backend.Models;
using bull_chat_backend.Models.DBase;
using bull_chat_backend.Services;
using bull_chat_backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace bull_chat_backend.Controllers
{
    [Route("api/[controller]/")]
    [ApiController]
    public class AuthenticationController(ILogger<AuthenticationController> logger,
        IUserAuthenticationService userRegistrationService,
        TokenMapService tokenMapService,
        IConfiguration configuration) : ControllerBase
    {
        private readonly ILogger<AuthenticationController> _logger = logger;
        private readonly IUserAuthenticationService _userAuthenticationService = userRegistrationService;
        private readonly TokenMapService _tokenMapService = tokenMapService;
        private readonly IConfiguration _configuration = configuration;

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterRequest request, CancellationToken token)
        {
            try
            {
                var user = await _userAuthenticationService.RegisterAsync(request.Login, request.Password, token);
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
        public async Task<IActionResult> Login([FromBody] UserLoginRequest request, CancellationToken token)
        {
            var loginResponse = await _userAuthenticationService.LoginAsync(request.Login, request.Password, token);

            var jwtToken = loginResponse.Token;
            var user = loginResponse.User;

            if (string.IsNullOrEmpty(jwtToken))
                return BadRequest("Бычек пал");

            Response.Cookies.Append(
                JwtAuthenticationExtensions.JwtCookieName,
                jwtToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddHours(_configuration.GetSection(nameof(JwtOptions)).Get<JwtOptions>()!.ExpiredHours)
                });
            return Ok(new
            {
                User = user.ToDto(),
                Tolen = jwtToken
            });
        }

        [HttpPost("logout")]
        public IActionResult Logout([UserFromRequest] User user)
        {
            _userAuthenticationService.Logout(user);
            return Ok("Бычек ушел");
        }

        [HttpPost("validate")]
        public IActionResult ValidateJwt([UserFromRequest] User user)
        {
            _tokenMapService.VerifyUserSession(user);
            return Ok(new
            {
                User = user.ToDto()
            });
        }

        [Authorize]
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok("Успешно!");
        }
    }
}
