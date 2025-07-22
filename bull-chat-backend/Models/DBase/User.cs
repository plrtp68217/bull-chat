using bull_chat_backend.Models.DTO;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace bull_chat_backend.Models.DBase

{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PasswordHash { get; set; }
        public ICollection<Message> Messages { get; set; } = [];
        // Для EF Core
        public User() { }

        public User(int id, string name, string passwordHash)
        {
            Id = id;
            Name = name;
            PasswordHash = passwordHash;
        }

        private readonly static User _empty = new(int.MinValue, string.Empty, string.Empty);
        [NotMapped] public static User Empty => _empty;
        public static bool IsEmpty(User user) => user.Id == Empty.Id;
        public UserDto ToDto() => new(Id, Name);
        public override int GetHashCode() => HashCode.Combine(Id, Name);
        public override string ToString() => $"Id = {Id} Name = {Name}";

    }
}
