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

        [NotMapped]
        public byte[] UserSessionHash
        {
            get
            {
                var combinedData = $"{Id}:{Name}:{PasswordHash}";
                return System.Security
                        .Cryptography.SHA256
                        .HashData(Encoding.UTF8.GetBytes(combinedData));

            }
        }

        [NotMapped]
        public string UserSessionHashString => Convert.ToBase64String(UserSessionHash);

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
        public UserDto ToDto(bool isAssignSessionHash = false) 
            => new(Id, Name, isAssignSessionHash ? Convert.ToBase64String(UserSessionHash) : "");
        public override int GetHashCode() => HashCode.Combine(Id, Name);
        public override string ToString() => $"Id = {Id} Name = {Name}";

    }
}
