using bull_chat_backend.Repository.RepositoryInterfaces;
using bull_chat_backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace bull_chat_backend.Hubs
{
    [Authorize]
    public record UserCredentials(string SessionHash, int UserId);
    public class ChatHub(
        IMessageRepository messageRepository,
        IUserRepository userRepository,
        TokenMapService tokenMapService,
        ILogger<ChatHub> logger) : Hub<IChatHub>
    {
        public const string HUB_URI = "/chatHub";
        private readonly ILogger<ChatHub> _logger = logger;
        private readonly IMessageRepository _messageRepository = messageRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly TokenMapService _tokenMapService = tokenMapService;

        public async Task SendMessage(UserCredentials userCredentials, string content)
        {
            var token = Context.ConnectionAborted;
            var user = await _userRepository.GetByIdAsync(userCredentials.UserId, token);
            var rawHash = Convert.FromBase64String(userCredentials.SessionHash);
            var isUserVerified = _tokenMapService.VerifyUserSession(user, rawHash);

            if (!isUserVerified)
            {
                _logger.LogWarning("Бычка не удалось найти в стойле (возможно, попытка обмана) hash = {UserHash}", rawHash);
                throw new UnauthorizedAccessException("Invalid session");
            }

            _logger.LogDebug("От бычка с именем {BullName} пришло {ContentText}", user.Name, content);

              
            var message = await _messageRepository.AddAsync(user,content,Models.DBase.Enum.ContentType.Text, token);
            var messageDto = message.ToDto();

            await Clients.All.ReceiveMessage(messageDto);
        }
    }
}
