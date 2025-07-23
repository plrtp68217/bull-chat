using bull_chat_backend.Models.DBase;
using bull_chat_backend.Models.DTO;

namespace bull_chat_backend.Repository.RepositoryInterfaces
{
    public interface IMessageRepository : IRepository<Message>
    {
        Task<Message> AddAsync(User user, string content, Models.DBase.Enum.ContentType contentType, CancellationToken token);
        ValueTask<DateTime> LastMessageDate(CancellationToken token);
        Task<ICollection<MessageDto>> GetPagedMessages(DateTime? cursor, bool isNext, CancellationToken token, int pageSize);
    }
}
