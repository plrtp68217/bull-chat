using bull_chat_backend.Models.DTO;

namespace bull_chat_backend.Models.DBase
{
    public class Message
    {
        public int Id { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;

        public int UserId { get; set; }

        public User User { get; set; }
        public Content Content { get; set; }
        public Message(User msgFrom, string text)
        {
            User = msgFrom;
            Content = new Content()
            {
                Item = text
            };
        }
        public Message() { }

        public MessageDto ToDto()
        {
            return new MessageDto()
            {
                Date = Date,
                User = User.ToDto(),
                Content = new ContentDto(Content!.Item!)
            };
        }
        public static Message Empty => new()
        {
            User = User.Empty,
            Content = Content.Empty,
        };
        public static bool IsEmpty(Message msg) => User.IsEmpty(msg.User!) && Content.IsEmpty(msg.Content!);
    }
}
