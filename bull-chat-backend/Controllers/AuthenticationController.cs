using bull_chat_backend.Extensions;
using bull_chat_backend.Models;
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
        private readonly IUserAuthenticationService _userAuthenticationService;
        private readonly IConfiguration _configuration;

        public AuthenticationController(ILogger<AuthenticationController> logger, IUserAuthenticationService userRegistrationService, IConfiguration configuration)
        {
            _logger = logger;
            _userAuthenticationService = userRegistrationService;
            _configuration = configuration;
        }

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

        [HttpPost("validate-jwt")]
        public async Task<IActionResult> ValidateJwt([FromBody] string jwtToken, CancellationToken token)
        {
            var validationResult = await _userAuthenticationService.ValidateTokenAsync(jwtToken, token);
            return Ok(validationResult);
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
                    // Токен генерируется на 24 ч, поэтому ставим жизнь куков Now + 24 (время жизни см. appsettings.json)
                    Expires = DateTimeOffset.UtcNow.AddHours(_configuration.GetSection(nameof(JwtOptions)).Get<JwtOptions>()!.ExpiredHours)
                });
            return Ok(new
            {
                Token = $"{jwtToken}",
                User = user.ToDto()
            });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            /*
             * TODO:    Инвалидировать токен.
             *          Сейчас токен бычка можно скопировать и войти по нему
             */
            HttpContext.Response.Cookies.Delete(JwtAuthenticationExtensions.JwtCookieName);
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
