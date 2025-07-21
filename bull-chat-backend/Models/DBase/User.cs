using bull_chat_backend.Models.DTO;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace bull_chat_backend.Models.DBase

{
    public class User
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? PasswordHash { get; set; }
        public ICollection<Message> Messages { get; set; } = [];

        private readonly static User _empty = new()
        {
            Id = int.MinValue,
            Name = string.Empty,
            PasswordHash = string.Empty,
            Messages = []
        };
        [NotMapped]
        public static User Empty { get => _empty; }
        public static bool IsEmpty(User user) => user.Id == Empty.Id;
        public UserDto ToDto() => new(Id, Name);
        public override int GetHashCode() => HashCode.Combine(Id, Name);
        public override string ToString() => $"Id = {Id} Name = {Name}";

    }
}
