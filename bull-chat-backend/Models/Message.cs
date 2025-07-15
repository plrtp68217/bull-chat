namespace bull_chat_backend.Models

{
    public class Message
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int UserId { get; set; }
        public int ContentId { get; set; }
        public User? User { get; set; }
        public Content? Content { get; set; }
    }
}
