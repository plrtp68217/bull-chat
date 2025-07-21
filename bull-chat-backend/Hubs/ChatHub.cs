using bull_chat_backend.Models.DBase;
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
        ILogger<ChatHub> logger) : Hub<IChatHub>
    {
        public const string HUB_URI = "/chatHub";
        private const string CURRENT_USER = "CurrentUser";
        private readonly ILogger<ChatHub> _logger = logger;
        private readonly IMessageRepository _messageRepository = messageRepository;

        public async Task SendMessage(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new HubException("Сообщение не может быть пустым");

            if (!Context.Items.TryGetValue(CURRENT_USER, out var currentUserObj) || currentUserObj is not User currentUser)
                throw new HubException("Бычек не найден в контексте подключения");

            _logger.LogDebug("От бычка с именем {BullName} пришло {ContentText}", currentUser.Name, content);

            var token = Context.ConnectionAborted;
            var message = await _messageRepository.AddAsync(currentUser, content, Models.DBase.Enum.ContentType.Text, token);

            token.ThrowIfCancellationRequested();

            var messageDto = message.ToDto();
            
            await Clients.AllExcept(Context.ConnectionId).ReceiveMessage(messageDto);
            await Clients.Caller.ReceiveMessage(messageDto.WithAuthorFlag(true));
        }
    }
}
