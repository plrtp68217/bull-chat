using bull_chat_backend.Models.DBase;
using bull_chat_backend.Models.DTO;

namespace bull_chat_backend.Repository.RepositoryInterfaces
{
    public interface IMessageRepository : IRepository<Message>
    {
        ValueTask<DateTime> LastMessageDate(CancellationToken token);
        Task<IList<MessageDto>> GetPagedMessages(DateTime cursor, int pageSize, CancellationToken token);
    }
}
