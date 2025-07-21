using bull_chat_backend.Models;
using bull_chat_backend.Models.DBase;
using bull_chat_backend.Repository.RepositoryInterfaces;
using bull_chat_backend.Services.Interfaces;

namespace bull_chat_backend.Services
{
    public class UserAuthenticationService : IUserAuthenticationService
    {
        private readonly ILogger<UserAuthenticationService> _logger;
        private readonly IJwtGenerator<User> _jwtGenerator;
        private readonly TokenMapService _tokenMap;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUserRepository _userRepository;

        public UserAuthenticationService(
            IPasswordHasher passwordHasher,
            IUserRepository userRepository,
            IJwtGenerator<User> jwtProvider,
            TokenMapService tokenMap,
            ILogger<UserAuthenticationService> logger)
        {
            _logger = logger;
            _jwtGenerator = jwtProvider;
            _tokenMap = tokenMap;
            _passwordHasher = passwordHasher;
            _userRepository = userRepository;
        }


        public async Task<LoginResponse> LoginAsync(string name, string password, CancellationToken token)
        {
            _logger.LogDebug("Попытка залогинить бычка {name}", name);
            var user = await _userRepository.GetByNameAsync(name, token);

            if (User.IsEmpty(user))
            {
                _logger.LogError("Бычек с именем {name} не найден!", name);
                return LoginResponse.Empty;

            }
            _logger.LogDebug("Бычек с именем {name} найден!", name);

            var isValidPassword = _passwordHasher.VerifySHA256(password, user.PasswordHash!);

            if (!isValidPassword)
            {
                _logger.LogError($"Ой-ой кто то быканул...");
                return LoginResponse.Empty;
            }
            var jwtToken = _jwtGenerator.GenerateToken(user);

            _tokenMap.AddUserSession(user,jwtToken);

            return new LoginResponse(jwtToken, user);
        }

        public void Logout(User user) => _tokenMap.RemoveUserSession(user);

        public async Task<User> RegisterAsync(string name, string password, CancellationToken token)
        {
            if (await _userRepository.IsExistByName(name, token))
                throw new InvalidDataException($"Бычек с именем {nameof(name)} уже в стойле");

            var hash = _passwordHasher.GetHashSHA256(password);

            var user = new User()
            {
                Name = name,
                PasswordHash = hash
            };

            await _userRepository.AddAsync(user, token);

            _logger.LogDebug("Бычек с именем {name} зарегистрирован", name);

            return user;
        }

        public async ValueTask<bool> ValidateTokenAsync(string jwtToken, CancellationToken token)
        {
            return await _jwtGenerator.ValidateTokenAsync(jwtToken);
        }

    }
}