namespace bull_chat_backend.Models.DTO
{
    public class MessageDto : IComparable<MessageDto>, IEquatable<MessageDto>
    {
        public MessageDto(int id, DateTime date, UserDto? user, ContentDto? content, bool isAuthor = false)
        {
            Id = id;
            Date = date;
            User = user;
            Content = content;
        }

        public int Id { get; set; }
        public DateTime Date { get; }
        public UserDto? User { get; }
        public ContentDto? Content { get; }

        public int CompareTo(MessageDto? other)
        {
            if (other == null) return 1;

            int dateComparison = Date.CompareTo(other.Date);
            if (dateComparison != 0) return dateComparison;

            return Id.CompareTo(other.Id);
        }

        public bool Equals(MessageDto? other)
        {
            if (other == null) return false;
            return Date == other.Date && Id == other.Id;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as MessageDto);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Date, Id);
        }

        public static bool operator ==(MessageDto? left, MessageDto? right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (left is null || right is null) return false;
            return left.Equals(right);
        }

        public static bool operator !=(MessageDto? left, MessageDto? right)
        {
            return !(left == right);
        }

        public static bool operator <(MessageDto? left, MessageDto? right)
        {
            if (left is null) return right is not null;
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(MessageDto? left, MessageDto? right)
        {
            if (left is null) return true;
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(MessageDto? left, MessageDto? right)
        {
            if (left is null) return false;
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(MessageDto? left, MessageDto? right)
        {
            if (left is null) return right is null;
            return left.CompareTo(right) >= 0;
        }
    }
}