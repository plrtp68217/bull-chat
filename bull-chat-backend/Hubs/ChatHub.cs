using bull_chat_backend.Models.DBase;
using bull_chat_backend.Repository.RepositoryInterfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace bull_chat_backend.Hubs
{
    public interface IChatHub 
    {
        Task SendMessage(string user, string text);
        Task ReceiveMessage(MessageDto message);
        Task NotifyTyping(string user);
        Task GetConnectedUsers();
    }

    public class MessageDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }

        public static MessageDto FromEntity(Message message)
        {
            return new MessageDto
            {
                Id = message.Id,
                UserId = message.User.Id,
                UserName = message.User.Name,
                Text = message.Content.Text,
                Date = message.Date
            };
        }
    }

    public class ChatHub : Hub<IChatHub>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IContentRepository _contentRepository;

        public ChatHub(
            IUserRepository userRepository,
            IMessageRepository messageRepository,
            IContentRepository contentRepository)
        {
            _userRepository = userRepository;
            _messageRepository = messageRepository;
            _contentRepository = contentRepository;
        }

        public async Task SendMessage(int userId, string text)
        {
            var cts = new CancellationTokenSource();
            CancellationToken token = cts.Token;

            User user = await _userRepository.GetByIdAsync(userId, token);

            if (user == null) throw new HubException("User not found");

            Message message = new()
            {
                UserId = userId,
                Date = DateTime.UtcNow,
                User = user,
                Content = new Content { Text = text },
            };

            await _messageRepository.AddAsync(message, token);

            await Clients.All.ReceiveMessage(MessageDto.FromEntity(message));
        }
    }
}
