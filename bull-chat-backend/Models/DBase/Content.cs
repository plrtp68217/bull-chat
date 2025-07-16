namespace bull_chat_backend.Models.DBase

{
    public class Content
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public Message? Message { get; set; }

        public static Content Empty => new()
        {
            Id = int.MinValue,
            Text = string.Empty,
            Message = Message.Empty,
        };
    }
}
