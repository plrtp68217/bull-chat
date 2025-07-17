using Microsoft.AspNetCore.Identity;

namespace bull_chat_backend.Models.DBase

{
    public class User
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string PasswordHash { get; set; }

        public ICollection<Message> Messages { get; set; } = [];

        public static readonly User Empty = new()
        {
            Id = int.MinValue,
            Name = string.Empty,
            PasswordHash = string.Empty,
            Messages = []
        };

        public override string ToString()
        {
            return $"{Id:D4} {Name}";
        }
        public static bool IsEmpty(User user) => user.Id == Empty.Id;
    }
}
