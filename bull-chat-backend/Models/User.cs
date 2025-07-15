namespace bull_chat_backend.Models

{
    public class User
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string PasswordHash { get; set; }

        public ICollection<Message> Messages { get; set; } = [];

        public static User Empty => new()
        {
            Name = string.Empty,
            PasswordHash = string.Empty,
            Id = int.MinValue,
            Messages = Enumerable.Empty<Message>().ToList()
        };
    }
}
