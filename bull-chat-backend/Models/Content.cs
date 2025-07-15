namespace bull_chat_backend.Models

{
    public class Content
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public Message? Message { get; set; }
    }
}
