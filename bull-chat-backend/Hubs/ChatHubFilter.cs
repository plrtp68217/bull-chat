using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using bull_chat_backend.Services;
using bull_chat_backend.Extensions;

namespace bull_chat_backend.Hubs
{
    public class ChatHubFilter(TokenMapService tokenMap) : IHubFilter
    {
        private readonly TokenMapService _tokenMapService = tokenMap;

        public async ValueTask<object?> InvokeMethodAsync(
            HubInvocationContext context,
            Func<HubInvocationContext, ValueTask<object?>> next)
        {
            var httpUser = context.Context.User;
            if (httpUser?.Identity is not { IsAuthenticated: true })
                throw new HubException("Бычек не авторизован");

            // Получаем ID пользователя из токена
            var userIdClaim = httpUser.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                throw new HubException("Идентификатор бычка не найден в токене");

            var jwtToken = context.Context.GetHttpContext()?.Request.Cookies[JwtAuthenticationExtensions.JwtCookieName];

            if (string.IsNullOrEmpty(jwtToken) || !_tokenMapService.IsTokenActive(jwtToken))
            {
                // Токен истечен
                throw new HubException("Токен недействителен или отозван (бычек спекся)");
            }

            // Привязываем текущего юзера в Items
            var user = _tokenMapService.GetUserByJwt(jwtToken);
            
            if (user != null)
            {
                context.Context.Items["CurrentUser"] = user;
            }

            return await next(context);
        }
    }
}