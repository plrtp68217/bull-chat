using bull_chat_backend.Models.DBase;
using bull_chat_backend.Repository.RepositoryInterfaces;
using bull_chat_backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace bull_chat_backend.Hubs
{
    [Authorize]
    public class ChatHub(
        IMessageRepository messageRepository,
        ILogger<ChatHub> logger) : Hub<IChatHub>
    {
        public const string HUB_URI = "/chatHub";
        private readonly ILogger<ChatHub> _logger = logger;
        private readonly IMessageRepository _messageRepository = messageRepository;

        public async Task SendMessage(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new HubException("Сообщение не может быть пустым");

            var currentUser = Context.GetCurrentUser();

            _logger.LogDebug("От бычка с именем {BullName} пришло {ContentText}", currentUser.Name, content);

            var token = Context.ConnectionAborted;
            var message = new Message(currentUser, DateTime.UtcNow, Models.DBase.Enum.ContentType.Text, content);
            await _messageRepository.AddAsync(message, token);

            token.ThrowIfCancellationRequested();

            await Clients.All.ReceiveMessage(message.ToDto());
        }
    }
}
