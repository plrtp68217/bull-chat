using bull_chat_backend.Models.DBase;
using bull_chat_backend.Repository.RepositoryInterfaces;
using Microsoft.AspNetCore.Components.Forms;
using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace bull_chat_backend.Services
{
    public class TokenMapService(ILogger<TokenMapService> logger)
    {
        private readonly ConcurrentDictionary<string, User> _jwtUserMap = new();
        private readonly ILogger<TokenMapService> _logger = logger;

        public bool AddUserSession(User user, string jwt, byte[] sessionHash)
        {
            if (user == null || string.IsNullOrEmpty(jwt) || sessionHash == null || sessionHash.Length == 0)
            {
                _logger.LogWarning("Была попытка создать невалидную сессию");
                return false;
            }

            _jwtUserMap[jwt] = user;

            _logger.LogInformation("Добавлена новая сессия для бычка с id = {UserId}", user.Id);
            return true;
        }

        public string GetJwtByUser(User user)
        {
            if (user == null || User.IsEmpty(user))
            {
                _logger.LogWarning("Попытка получить JWT для невалидного пользователя");
                return string.Empty;
            }

            var jwt = _jwtUserMap.FirstOrDefault(pair => pair.Value.Id == user.Id).Key;

            if (jwt == null)
            {
                _logger.LogWarning("JWT для пользователя с id = {UserId} не найден", user.Id);
            }
            else
            {
                _logger.LogDebug("Найден JWT для пользователя с id = {UserId}", user.Id);
            }

            return jwt ?? string.Empty;
        }
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

        public bool RemoveUserSession(User user)
        {
            if (user == null || User.IsEmpty(user))
                throw new ArgumentNullException(nameof(user));

            bool removed = false;

            // Remove JWT
            var jwtToRemove = _jwtUserMap.FirstOrDefault(pair => pair.Value.Id == user.Id).Key;
            if (jwtToRemove != null)
            {
                removed |= _jwtUserMap.TryRemove(jwtToRemove, out _);
            }


            return removed;
        }
        
        public void ClearAllSessions()
        {
            _jwtUserMap.Clear();
            _logger.LogInformation("Все сессии очищены");
        }

        public bool IsTokenActive(string jwt)
        {
            if (string.IsNullOrEmpty(jwt) || !_jwtUserMap.ContainsKey(jwt))
                return false;

            if (IsJwtExpired(jwt))
            {
                var delResult = _jwtUserMap.TryRemove(jwt, out var user);
                if (delResult) _logger.LogInformation("Бычок вышвырнут {User}", user);
                return false;
            }

            return true;
        }
        private bool IsJwtExpired(string jwt)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(jwt);
                return token.ValidTo < DateTime.UtcNow;
            }
            catch
            {
                return true;
            }
        }
    }
}