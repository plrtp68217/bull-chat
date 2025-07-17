using bull_chat_backend.Models.DBase;
using bull_chat_backend.Models.DTO;
using bull_chat_backend.Repository;
using bull_chat_backend.Repository.RepositoryInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace bull_chat_backend.Hubs
{
    [Authorize]
    public class ChatHub(
        IUserRepository userRepository,
        IMessageRepository messageRepository,
        ILogger<ChatHub> logger) : Hub<IChatHub>
    {
        public const string HUB_URI = "/chat";
        private readonly ILogger<ChatHub> _logger = logger;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IMessageRepository _messageRepository = messageRepository;

        public async Task SendMessage(int userId, string content)
        {
            var token = Context.ConnectionAborted;

            var user = await _userRepository.GetByIdAsync(userId, token) ?? throw new HubException("User not found");

            _logger.LogDebug("От бычка с именем {BullName} пришло {ContentText}", user.Name, content);

              
            var message = await _messageRepository.AddAsync(user,content,Models.DBase.Enum.ContentType.Text, token);
            var messageDto = message.ToDto();

            await Clients.All.ReceiveMessage(messageDto);
        }
    }
}
