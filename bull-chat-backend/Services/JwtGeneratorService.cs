using bull_chat_backend.Models;
using bull_chat_backend.Models.DBase;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace bull_chat_backend.Services
{
    public class JwtGeneratorService(IOptions<JwtOptions> options, ILogger<JwtGeneratorService> logger)
    {
        private readonly ILogger<JwtGeneratorService> _logger = logger;
        private readonly JwtOptions _options = options.Value;

        private const string DEFAULT_ROLE = "user";
        public string GenerateToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, DEFAULT_ROLE)

            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                audience: _options.Audience,
                issuer: _options.Issuer,
                signingCredentials: signingCredentials,
                expires: DateTime.UtcNow.AddHours(_options.ExpiredHours)
                );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            _logger.LogInformation("JWT выдан для пользователя: {UserName} (ID: {UserId})", user.Name, user.Id);
            return tokenString;

        }
    }
}
