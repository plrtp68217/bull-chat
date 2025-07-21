using bull_chat_backend.Models.DBase;
using Microsoft.AspNetCore.Components.Forms;
using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;

namespace bull_chat_backend.Services
{
    public class TokenMapService(ILogger<TokenMapService> logger)
    {
        private readonly ConcurrentDictionary<string, User> _jwtUserMap = new();
        private readonly ConcurrentDictionary<User, byte[]> _userSessionHashMap = new();
        private readonly ILogger<TokenMapService> _logger = logger;

        public bool AddUserSession(User user, string jwt, byte[] sessionHash)
        {
            if (user == null || string.IsNullOrEmpty(jwt) || sessionHash == null || sessionHash.Length == 0)
            {
                _logger.LogWarning("Была попытка создать невалидную сессию");
                return false;
            }

            _jwtUserMap[jwt] = user;
            _userSessionHashMap[user] = sessionHash;

            _logger.LogInformation("Добавлена новая сессия для бычка с id = {UserId}", user.Id);
            return true;
        }

        public bool VerifyUserSession(User user, byte[] inputHash)
        {
            if (user == null ||
                inputHash == null ||
                User.IsEmpty(user) ||
                inputHash.Length == 0)
                return false;

            if (!_userSessionHashMap.TryGetValue(user, out var storedHash))
                return false;

            return CryptographicOperations.FixedTimeEquals(storedHash, inputHash);
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

            // Remove hash
            removed |= _userSessionHashMap.TryRemove(user, out _);

            if (removed)
            {
                _logger.LogInformation("Из сессии удален бычек с id = {UserId}", user.Id);
            }

            return removed;
        }

        public User GetUserByUserSessionHash(byte[] userHash)
        {
            if (userHash == null || userHash.Length == 0)
                return User.Empty;

            return _userSessionHashMap.FirstOrDefault(pair => CryptographicOperations.FixedTimeEquals(pair.Value, userHash)).Key ?? User.Empty;
        }
        public void ClearAllSessions()
        {
            _jwtUserMap.Clear();
            _userSessionHashMap.Clear();
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