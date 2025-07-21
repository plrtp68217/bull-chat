using bull_chat_backend.Repository.RepositoryInterfaces;
using bull_chat_backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace bull_chat_backend.Hubs
{
    [Authorize]
    public class ChatHub(
        IMessageRepository messageRepository,
        TokenMapService tokenMapService,
        ILogger<ChatHub> logger) : Hub<IChatHub>
    {
        public const string HUB_URI = "/chatHub";
        private readonly ILogger<ChatHub> _logger = logger;
        private readonly IMessageRepository _messageRepository = messageRepository;
        private readonly TokenMapService _tokenMapService = tokenMapService;

        public async Task SendMessage(string clientHash, string content)
        {
            var token = Context.ConnectionAborted;
            var rawHash = Convert.FromBase64String(clientHash);
            var user = _tokenMapService.GetUserByUserSessionHash(rawHash);

            if (!_tokenMapService.VerifyUserSession(user, rawHash))
            {
                _logger.LogWarning("Бычка не удалось найти в стойле hash = {UserHash}", clientHash);
                throw new UnauthorizedAccessException("Invalid session");
            }

            _logger.LogDebug("От бычка с именем {BullName} пришло {ContentText}", user.Name, content);

              
            var message = await _messageRepository.AddAsync(user,content,Models.DBase.Enum.ContentType.Text, token);
            var messageDto = message.ToDto();

            await Clients.All.ReceiveMessage(messageDto);
        }
    }
}
