using bull_chat_backend.Models.DBase;
using bull_chat_backend.Repository.RepositoryInterfaces;
using bull_chat_backend.Services.Interfaces;
using System.Threading.Tasks;

namespace bull_chat_backend.Services
{
    public class UserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly IJwtGenerator<User> _jwtProvider;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUserRepository _userRepository;

        public UserService(
            IPasswordHasher passwordHasher,
            IUserRepository userRepository,
            IJwtGenerator<User> jwtProvider,
            ILogger<UserService> logger)
        {
            _logger = logger;
            _jwtProvider = jwtProvider;
            _passwordHasher = passwordHasher;
            _userRepository = userRepository;
        }


        public async Task<string> LoginAsync(string name, string password, CancellationToken token)
        {
            _logger.LogDebug("Попытка залогинить бычка {name}", name);
            var user = await _userRepository.GetByNameAsync(name , token);

            if (User.IsEmpty(user))
            {
                _logger.LogError("Бычек с именем {name} не найден!", name);
                return string.Empty;

            }
            _logger.LogDebug("Бычек с именем {name} найден!", name);

            var isValidPassword = _passwordHasher.Verify(password, user.PasswordHash);

            if (!isValidPassword)
            {
                _logger.LogError($"Ой-ой кто то быканул...");
                return string.Empty;
            }

            return _jwtProvider.GenerateToken(user);

        }
        public async Task<User> RegisterAsync(string name, string password , CancellationToken token)
        {
            var hash = _passwordHasher.GetHash(password);

            var user = new User()
            {
                Name = name,
                PasswordHash = hash
            };

            await _userRepository.AddAsync(user, token);

            _logger.LogDebug("Бычек с именем {name} зарегистрирован", name);

            return user;
        }


    }
}