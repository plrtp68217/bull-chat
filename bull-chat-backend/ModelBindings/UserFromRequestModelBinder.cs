using bull_chat_backend.Extensions;
using bull_chat_backend.Services;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace bull_chat_backend.ModelBindings
{
    /// <summary>
    /// Модельный биндер, извлекающий пользователя из JWT-токена, переданного в заголовке Authorization.
    /// Используется в сочетании с атрибутом <c>[UserFromRequest]</c> для автоматической подстановки <see cref="User"/>.
    /// </summary>
    public class UserFromRequestModelBinder(TokenMapService tokenMapService, ILogger<UserFromRequestModelBinder> logger) : IModelBinder
    {
        private readonly TokenMapService _tokenMapService = tokenMapService;
        private readonly ILogger<UserFromRequestModelBinder> _logger = logger;

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var httpContext = bindingContext.HttpContext;
            var httpUser = httpContext.User;

            // Проверка аутентификации
            if (httpUser?.Identity is not { IsAuthenticated: true })
            {
                bindingContext.ModelState.AddModelError("User", "Бычек не авторизован");
                _logger.LogWarning("Была попытка дернуться у бычка, но был получен жесткий удар в пятак");
                return Task.CompletedTask;
            }

            // Получение JWT из заголовка Authorization
            var authHeader = httpContext.Request.Headers.Authorization.ToString();

            if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                bindingContext.ModelState.AddModelError("User", "Заголовок Authorization отсутствует или некорректен");
                _logger.LogWarning("Бычок не принёс Bearer токен");
                return Task.CompletedTask;
            }

            var jwt = authHeader["Bearer ".Length..].Trim();

            if (string.IsNullOrEmpty(jwt) || !_tokenMapService.IsTokenActive(jwt))
            {
                bindingContext.ModelState.AddModelError("User", "Токен бычка недействителен или отозван");
                _logger.LogInformation("Пятяк просрочил токен");
                return Task.CompletedTask;
            }

            var user = _tokenMapService.GetUserByJwt(jwt);

            if (!_tokenMapService.VerifyUserSession(user))
            {
                bindingContext.ModelState.AddModelError("User", "Сессия бычка завершена (Logout)");
                _logger.LogInformation("Пятяк свой токен удалил, но пытается брыкаться (ДУРАК??)");
                return Task.CompletedTask;
            }

            bindingContext.Result = ModelBindingResult.Success(user);
            return Task.CompletedTask;
        }
    }
}
