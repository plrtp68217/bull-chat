using bull_chat_backend.Hubs;
using bull_chat_backend.Models;
using bull_chat_backend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace bull_chat_backend.Extensions
{
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
                            // Для SignalR - из query string
                            if (context.Request.Path.StartsWithSegments(ChatHub.HUB_URI))
                                context.Token = context.Request.Query["access_token"];


                            // Для обычных запросов - из куки
                            context.Token ??= context.Request.Cookies[JwtCookieName];

                            return Task.CompletedTask;
                        },
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

                            //var user = tokenMap.GetUserByJwt(jwtToken);
                            //if (user != null)
                            //{
                            //    context.HttpContext.Items["CurrentUser"] = user;
                            //}
                        }

                    };
                });

            return services;
        }
    }
}