using bull_chat_backend.Models.DBase.Enum;
using bull_chat_backend.Models.DTO;
using System.ComponentModel.DataAnnotations.Schema;

namespace bull_chat_backend.Models.DBase
{
    public class Message : IComparable<Message>, IEquatable<Message>
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }
        public Content Content { get; set; }

        // Для EF Core
        public Message() { }
        public Message(User msgFrom, DateTime date, ContentType contentType, string text)
        {
            Date = date;
            User = msgFrom;
            Content = new Content(text, contentType);
        }
        public MessageDto ToDto() => new(Id, Date, User.ToDto(), Content.ToDto());
        public static bool IsEmpty(Message msg) => User.IsEmpty(msg.User!) && Content.IsEmpty(msg.Content!);

        private static readonly Message _empty = new(User.Empty, DateTime.MinValue, ContentType.Text, string.Empty);
        [NotMapped] public static Message Empty => _empty;

        public int CompareTo(Message other)
        {
            if (other == null) return 1;

            int dateComparison = Date.CompareTo(other.Date);
            if (dateComparison != 0) return dateComparison;

            return Id.CompareTo(other.Id);
        }

        public bool Equals(Message other)
        {
            if (other == null) return false;
            return Date == other.Date && Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Message);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Date, Id);
        }

        public static bool operator ==(Message left, Message right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (left is null || right is null) return false;
            return left.Equals(right);
        }

        public static bool operator !=(Message left, Message right)
        {
            return !(left == right);
        }

        public static bool operator <(Message left, Message right)
        {
            if (left is null) return right is not null;
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(Message left, Message right)
        {
            if (left is null) return true;
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(Message left, Message right)
        {
            if (left is null) return false;
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(Message left, Message right)
        {
            if (left is null) return right is null;
            return left.CompareTo(right) >= 0;
        }
    }
}