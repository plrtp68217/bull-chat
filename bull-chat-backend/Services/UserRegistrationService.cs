using bull_chat_backend.Models.DBase;
using bull_chat_backend.Repository.RepositoryInterfaces;
using bull_chat_backend.Services.Interfaces;

namespace bull_chat_backend.Services
{
    public class UserRegistrationService : IUserRegistrationService
    {
        private readonly ILogger<UserRegistrationService> _logger;
        private readonly IJwtGenerator<User> _jwtProvider;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUserRepository _userRepository;

        public UserRegistrationService(
            IPasswordHasher passwordHasher,
            IUserRepository userRepository,
            IJwtGenerator<User> jwtProvider,
            ILogger<UserRegistrationService> logger)
        {
            _logger = logger;
            _jwtProvider = jwtProvider;
            _passwordHasher = passwordHasher;
            _userRepository = userRepository;
        }


        public async Task<ILoginResponse> LoginAsync(string name, string password, CancellationToken token)
        {
            _logger.LogDebug("Попытка залогинить бычка {name}", name);
            var user = await _userRepository.GetByNameAsync(name, token);

            if (User.IsEmpty(user))
            {
                _logger.LogError("Бычек с именем {name} не найден!", name);
                return new LoginResponse("", User.Empty);

            }
            _logger.LogDebug("Бычек с именем {name} найден!", name);

            var isValidPassword = _passwordHasher.Verify(password, user.PasswordHash);

            if (!isValidPassword)
            {
                _logger.LogError($"Ой-ой кто то быканул...");
                return new LoginResponse("", User.Empty);
            }

            var jwtToken = _jwtProvider.GenerateToken(user);

            return new LoginResponse(jwtToken, user);
        }

        public async Task<User> RegisterAsync(string name, string password, CancellationToken token)
        {
            if (await _userRepository.IsExistByName(name, token))
                throw new InvalidDataException($"Бычек с именем {nameof(name)} уже в стойле");

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