namespace bull_chat_backend.Hubs
{
    public interface IChatHub 
    {
        Task SendMessage(string user, string text);
        Task ReceiveMessage(MessageDto message);
        Task NotifyTyping(string user);
        Task GetConnectedUsers();
    }
}
