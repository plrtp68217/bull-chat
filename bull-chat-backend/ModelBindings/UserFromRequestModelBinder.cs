using bull_chat_backend.Extensions;
using bull_chat_backend.Services;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace bull_chat_backend.ModelBindings
{
    /// <summary>
    /// Модельный биндер, извлекающий пользователя из JWT-токена в cookie.
    /// Используется в сочетании с атрибутом <c>[UserFromRequest]</c> для автоматической подстановки <see cref="User"/>.
    /// </summary>
    public class UserFromRequestModelBinder(TokenMapService tokenMapService) : IModelBinder
    {
        private readonly TokenMapService _tokenMapService = tokenMapService;

        /// <summary>
        /// Выполняет биндинг модели <see cref="User"/> из JWT-токена, переданного в куки.
        /// </summary>
        /// <param name="bindingContext">Контекст биндинга ASP.NET Core.</param>
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var httpContext = bindingContext.HttpContext;
            var httpUser = httpContext.User;

            // Проверка аутентификации
            if (httpUser?.Identity is not { IsAuthenticated: true })
            {
                bindingContext.ModelState.AddModelError("User", "Бычек не авторизован");
                return Task.CompletedTask;
            }

            // Получение JWT из куки
            var jwt = httpContext.Request.Cookies[JwtAuthenticationExtensions.JwtCookieName];

            if (string.IsNullOrEmpty(jwt) || !_tokenMapService.IsTokenActive(jwt))
            {
                bindingContext.ModelState.AddModelError("User", "Токен бычка недействителен или отозван");
                return Task.CompletedTask;
            }

            var user = _tokenMapService.GetUserByJwt(jwt);

            if (!_tokenMapService.VerifyUserSession(user))
            {
                bindingContext.ModelState.AddModelError("User", "Сессия бычка завершена (Logout)");
                return Task.CompletedTask;
            }

            // Успешный результат биндинга
            bindingContext.Result = ModelBindingResult.Success(user);
            return Task.CompletedTask;
        }
    }
}
