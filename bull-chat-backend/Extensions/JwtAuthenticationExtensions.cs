using bull_chat_backend.Hubs;
using bull_chat_backend.Models;
using bull_chat_backend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace bull_chat_backend.Extensions
{

    /*
                                                ┌───────────────────────────┐
                                                │      HTTP-запрос          │
                                                │  (с куки или query токен) │
                                                └────────────┬──────────────┘
                                                             │
                                                             ▼
                                                ┌───────────────────────────┐
                                                │ UseAuthentication()       │
                                                │  └─ JwtBearerHandler      │
                                                │     └─ OnMessageReceived: │
                                                │        ├─ Извлекаем токен │
                                                │        │   ├─ из query    │
                                                │        │   └─ или из куки │
                                                └────────────┬──────────────┘
                                                             │
                                                             ▼
                                                ┌─────────────────────────────┐
                                                │  TokenValidationParameters  │
                                                │  └─ Проверка:               │
                                                │     ├─ Подпись              │
                                                │     ├─ Issuer / Audience    │
                                                │     ├─ Lifetime             │
                                                └────────────┬────────────────┘
                                                             │
                                                             ▼
                                                ┌─────────────────────────────┐
                                                │       OnTokenValidated      │
                                                └────────────┬────────────────┘
                                                             │
                                                             ▼
                                                ┌───────────────────────────┐
                                                │ UseAuthorization()        │
                                                │  └─ [Authorize]           │
                                                │     ├─ Проверка есть ли   │
                                                │     │   ClaimsPrincipal   │
                                                │     └─ Роли/Политики?     │
                                                └────────────┬──────────────┘
                                                             │
                                                             ▼
                                                ┌───────────────────────────┐
                                                │                           │
                                                │   Контроллер / Хендлер    │
                                                │                           │
                                                └───────────────────────────┘
    */

    public static class JwtAuthenticationExtensions
    {
        public const string JwtCookieName = "JWT_TOKEN";

        public static IServiceCollection ConfigureJwtOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtOptions>(configuration.GetSection(nameof(JwtOptions)));
            return services;
        }

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtOptions = configuration.GetSection(nameof(JwtOptions)).Get<JwtOptions>()
                ?? throw new InvalidOperationException("JWT configuration not found");


            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtOptions.Issuer,
                        ValidAudience = jwtOptions.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
                        LogValidationExceptions = true,
                        ClockSkew = TimeSpan.Zero
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var request = context.Request;
                            var tokenMap = context.HttpContext.RequestServices.GetRequiredService<TokenMapService>();

                            string? token = string.Empty;

                            var authHeader = request.Headers.Authorization.ToString();
                            if (authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                            {
                                token = authHeader["Bearer ".Length..].Trim();
                            }
                            else if (request.Path.StartsWithSegments(ChatHub.HUB_URI) &&
                                     request.Query.TryGetValue("access_token", out var accessToken))
                            {
                                token = accessToken.ToString();
                            }

                            if (string.IsNullOrEmpty(token) || !tokenMap.IsTokenActive(token))
                            {
                                context.Fail("Токен бычека не действителен");
                            }
                            else
                            {
                                context.Token = token;
                            }

                            return Task.CompletedTask;
                        },
                    };
                });

            return services;
        }
    }
}