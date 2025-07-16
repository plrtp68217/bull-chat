namespace bull_chat_backend.Models.DBase

{
    public class Message
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int UserId { get; set; }
        public int ContentId { get; set; }
        public User? User { get; set; }
        public Content? Content { get; set; }

        public static Message Empty => new()
        {
            Id = int.MinValue,
            Date = new DateTime().Date,
            UserId = int.MinValue,
            ContentId = int.MinValue,
            User = User.Empty,
            Content = Content.Empty,
        };
    }
}
