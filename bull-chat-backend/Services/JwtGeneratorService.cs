﻿using bull_chat_backend.Models;
using bull_chat_backend.Models.DBase;
using bull_chat_backend.Services.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace bull_chat_backend.Services
{
    public class JwtGeneratorService(IOptions<JwtOptions> options, ILogger<JwtGeneratorService> logger) : IJwtGenerator<User>
    {
        private readonly ILogger<JwtGeneratorService> _logger = logger;
        private readonly JwtOptions _options = options.Value;

        private const string DEFAULT_ROLE = "user";

        public string GenerateToken(User user)
        {
            if (string.IsNullOrWhiteSpace(_options.SecretKey))
            {
                _logger.LogError("SecretKey в JwtOptions не задан.");
                throw new InvalidOperationException("JWT SecretKey не задан в конфигурации.");
            }


            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, DEFAULT_ROLE)

            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));
            var keyBytes = Encoding.UTF8.GetBytes(_options.SecretKey);
            if (keyBytes.Length < 32)
            {
                throw new InvalidOperationException("JWT SecretKey должен быть не менее 32 байт (256 бит) длиной.");
            }
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                audience: _options.Audience,
                issuer: _options.Issuer,
                signingCredentials: signingCredentials,
                expires: DateTime.UtcNow.AddHours(_options.ExpiredHours)
                );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token); //здесь
            _logger.LogInformation("JWT выдан для пользователя: {UserName} (ID: {UserId})", user.Name, user.Id);
            return tokenString;

        }
    }
}
