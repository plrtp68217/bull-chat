using Microsoft.AspNetCore.SignalR;
using bull_chat_backend.Services;
using bull_chat_backend.Models.DBase;

namespace bull_chat_backend.Hubs.Fitler
{
    public class ChatHubAuthenticationFilter(
        SessionService tokenMap, 
        ILogger<ChatHubAuthenticationFilter> logger) : IHubFilter
    {
        private readonly SessionService _tokenMapService = tokenMap;
        private readonly ILogger<ChatHubAuthenticationFilter> _logger = logger;
        public const string CURRENT_USER = "CurrentUser";
        private const string URI_PARAM_NAME = "access_token";

        public async ValueTask<object?> InvokeMethodAsync(
            HubInvocationContext context,
            Func<HubInvocationContext, ValueTask<object?>> next)
        {
            var httpContext = context.Context.GetHttpContext();
            if (httpContext is null)
            {
                _logger.LogError("HTTP context null");
                throw new HubException("HTTP context is null");
            }

            var httpUser = context.Context.User;

            if (httpUser?.Identity is not { IsAuthenticated: true })
            {
                _logger.LogError("Попытка вызова метода неавторизованным бычеком (был вышвырнут)");
                throw new HubException("Бычек не авторизован");
            }

            string jwtToken = string.Empty;
            if (httpContext.Request.Path.StartsWithSegments(ChatHub.HUB_URI) &&
                httpContext.Request.Query.TryGetValue(URI_PARAM_NAME, out var accessToken))
            {
                jwtToken = accessToken.ToString();
            }

            if (string.IsNullOrEmpty(jwtToken) || _tokenMapService.IsTokenNotActive(jwtToken))
            {
                _logger.LogError("Недействительный или отозванный токен: {Token}", jwtToken);
                throw new HubException("Токен недействителен или отозван (бычек спекся)");
            }

            var user = _tokenMapService.GetUserByJwt(jwtToken);

            if (user == null)
            {
                _logger.LogError("Бычек по токену не найден: {Token}", jwtToken);
                throw new HubException("Бычек не найден");
            }

            context.Context.Items[CURRENT_USER] = user;

            return await next(context);
        }
    }

    public static class HubCallerContextExtensions
    {
        public static User UserFromContext(this HubCallerContext context)
        {
            if (context.Items.TryGetValue(ChatHubAuthenticationFilter.CURRENT_USER, out var value) && value is User user)
                return user;

            throw new HubException("Бычек не найден в контексте подключения");
        }
    }
}