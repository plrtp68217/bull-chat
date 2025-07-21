using bull_chat_backend.Models.DBase;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;

namespace bull_chat_backend.Services
{
    /// <summary>
    /// Сервис управления сессиями пользователей и маппингом JWT на пользователей.
    /// Хранит активные JWT-токены и связанные с ними объекты <see cref="User"/>.
    /// </summary>
    public class TokenMapService(ILogger<TokenMapService> logger)
    {
        private readonly ConcurrentDictionary<string, User> _jwtUserMap = new();
        private readonly ILogger<TokenMapService> _logger = logger;

        /// <summary>
        /// Добавляет сессию пользователя в карту токенов.
        /// </summary>
        /// <param name="user">Пользователь.</param>
        /// <param name="jwt">JWT токен.</param>
        /// <param name="sessionHash">Хэш сессии (не используется в текущей реализации, но зарезервировано).</param>
        /// <returns><c>true</c>, если успешно добавлено; иначе <c>false</c>.</returns>
        public bool AddUserSession(User user, string jwt)
        {
            if (user == null || string.IsNullOrEmpty(jwt))
            {
                _logger.LogWarning("Была попытка создать невалидную сессию");
                return false;
            }

            // Удаляем старую сессию пользователя (если есть)
            var existingJwt = _jwtUserMap.FirstOrDefault(pair => pair.Value.Id == user.Id).Key;
            if (!string.IsNullOrEmpty(existingJwt))
            {
                _jwtUserMap.TryRemove(existingJwt, out _);
                _logger.LogInformation("Старая сессия для пользователя {UserId} удалена при добавлении новой", user.Id);
            }

            // Добавляем новую сессию
            _jwtUserMap[jwt] = user;
            _logger.LogInformation("Добавлена новая сессия для бычка с id = {UserId}", user.Id);
            return true;
        }

        /// <summary>
        /// Получает пользователя, связанного с переданным JWT.
        /// </summary>
        /// <param name="jwt">JWT токен.</param>
        /// <returns>Объект пользователя, либо <see cref="User.Empty"/>, если не найден.</returns>
        public User GetUserByJwt(string jwt)
        {
            if (string.IsNullOrEmpty(jwt))
            {
                _logger.LogWarning("Попытка получить пользователя по пустому JWT");
                return User.Empty;
            }

            if (_jwtUserMap.TryGetValue(jwt, out var user))
            {
                _logger.LogDebug("Пользователь найден по JWT: {UserId}", user.Id);
                return user;
            }

            _logger.LogWarning("JWT не найден в сессии: {Jwt}", jwt);
            return User.Empty;
        }

        /// <summary>
        /// Удаляет сессию пользователя.
        /// </summary>
        /// <param name="user">Пользователь, чья сессия должна быть удалена.</param>
        /// <returns><c>true</c>, если сессия была удалена; иначе <c>false</c>.</returns>
        public bool RemoveUserSession(User user)
        {
            if (user == null || User.IsEmpty(user))
                throw new ArgumentNullException(nameof(user));

            bool removed = false;

            // Поиск JWT по пользователю
            var jwtToRemove = _jwtUserMap.FirstOrDefault(pair => pair.Value.Id == user.Id).Key;
            if (!string.IsNullOrEmpty(jwtToRemove))
            {
                removed = _jwtUserMap.TryRemove(jwtToRemove, out _);
                if (removed)
                    _logger.LogInformation("Сессия пользователя {UserId} удалена", user.Id);
            }

            return removed;
        }

        /// <summary>
        /// Очищает все активные сессии.
        /// </summary>
        public void ClearAllSessions()
        {
            _jwtUserMap.Clear();
            _logger.LogInformation("Все сессии очищены");
        }

        /// <summary>
        /// Проверяет, активна ли сессия пользователя.
        /// </summary>
        /// <param name="user">Пользователь.</param>
        /// <returns><c>true</c>, если пользователь имеет действующую сессию; иначе <c>false</c>.</returns>
        public bool VerifyUserSession(User user)
        {
            if (user == null || User.IsEmpty(user))
            {
                _logger.LogWarning("Попытка верификации пустого или невалидного пользователя");
                return false;
            }

            // Ищем JWT по пользователю
            var jwt = _jwtUserMap.FirstOrDefault(pair => pair.Value.Id == user.Id).Key;
            if (string.IsNullOrEmpty(jwt))
            {
                _logger.LogWarning("Сессия для пользователя {UserId} не найдена", user.Id);
                return false;
            }

            // Проверяем срок действия и наличие в словаре
            if (!IsTokenActive(jwt))
            {
                _logger.LogWarning("Сессия для пользователя {UserId} недействительна или токен истёк", user.Id);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Проверяет, действителен ли токен: он должен быть в карте и не должен быть просрочен.
        /// </summary>
        /// <param name="jwt">JWT токен.</param>
        /// <returns><c>true</c>, если токен активен и действителен; иначе <c>false</c>.</returns>
        public bool IsTokenActive(string jwt)
        {
            if (string.IsNullOrEmpty(jwt) || !_jwtUserMap.ContainsKey(jwt))
                return false;

            if (IsJwtExpired(jwt))
            {
                var removed = _jwtUserMap.TryRemove(jwt, out var user);
                if (removed)
                    _logger.LogInformation("Бычок вышвырнут: {User}", user);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Проверяет, истёк ли срок действия JWT.
        /// </summary>
        /// <param name="jwt">JWT токен.</param>
        /// <returns><c>true</c>, если токен просрочен или некорректен; иначе <c>false</c>.</returns>
        private bool IsJwtExpired(string jwt)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(jwt);
                return token.ValidTo < DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при проверке срока действия JWT");
                return true; // Считаем просроченным, если не удалось распарсить
            }
        }
    }
}
