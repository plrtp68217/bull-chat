using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using bull_chat_backend.Services;
using bull_chat_backend.Extensions;
using bull_chat_backend.Models.DBase;
using Microsoft.Extensions.Logging;

namespace bull_chat_backend.Hubs
{
    public class ChatHubAuthenticationFilter(
        TokenMapService tokenMap, 
        ILogger<ChatHubAuthenticationFilter> logger) : IHubFilter
    {
        private readonly TokenMapService _tokenMapService = tokenMap;
        private readonly ILogger<ChatHubAuthenticationFilter> _logger = logger;
        public const string CURRENT_USER = "CurrentUser";

        public async ValueTask<object?> InvokeMethodAsync(
            HubInvocationContext context,
            Func<HubInvocationContext, ValueTask<object?>> next)
        {
            var httpUser = context.Context.User;

            if (httpUser?.Identity is not { IsAuthenticated: true })
            {
                _logger.LogWarning("Попытка вызова метода неавторизованным бычеком (был вышвырнут)");
                throw new HubException("Бычек не авторизован");
            }

            var jwtToken = context.Context.GetHttpContext()?.Request.Cookies[JwtAuthenticationExtensions.JwtCookieName];

            if (string.IsNullOrEmpty(jwtToken) || !_tokenMapService.IsTokenActive(jwtToken))
            {
                _logger.LogWarning("Недействительный или отозванный токен: {Token}", jwtToken);
                throw new HubException("Токен недействителен или отозван (бычек спекся)");
            }

            var user = _tokenMapService.GetUserByJwt(jwtToken);

            if (user == null)
            {
                _logger.LogWarning("Бычек по токену не найден: {Token}", jwtToken);
                throw new HubException("Бычек не найден");
            }

            context.Context.Items[CURRENT_USER] = user;

            return await next(context);
        }
    }

    public static class HubCallerContextExtensions
    {
        public static User GetCurrentUser(this HubCallerContext context)
        {
            if (context.Items.TryGetValue(ChatHubAuthenticationFilter.CURRENT_USER, out var value) &&
                value is User user)
            {
                return user;
            }

            throw new HubException("Бычек не найден в контексте подключения");
        }
    }
}