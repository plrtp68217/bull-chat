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
                    // 2.   Проверяется что токен валидный
                    //      Тут же и проверяется его время жизни.
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
                        // 1.   Вытаскивается токен из Url или Cookie
                        OnMessageReceived = context =>
                        {
                            context.Token ??= context.Request.Cookies[JwtCookieName];

                            return Task.CompletedTask;
                        },
                        // 3.   Токен валиден, проверятся не удален ли токен досрочно (Logout) и есть ли сессия.
                        // 4.   Если все проверилось, то вызывается метод контроллера.
                        //          [Authorize] Атрибут может проверить на РОЛИ.
                        OnTokenValidated = context =>
                        {
                            var jwtToken = context.HttpContext.Request.Cookies[JwtCookieName];

                            if (string.IsNullOrEmpty(jwtToken))
                            {
                                context.Fail("Token not found in cookies");
                                return Task.CompletedTask;
                            }

                            var tokenMap = context.HttpContext.RequestServices.GetRequiredService<TokenMapService>();

                            if (!tokenMap.IsTokenActive(jwtToken))
                            {
                                context.Fail("Token is not active or has been revoked");
                                return Task.CompletedTask;
                            }
                            return Task.CompletedTask;
                        }

                    };
                });

            return services;
        }
    }
}