using bull_chat_backend.Models.DBase;
using bull_chat_backend.Models.DTO;

namespace bull_chat_backend.Repository.RepositoryInterfaces
{
    public interface IMessageRepository : IRepository<Message>
    {
        ValueTask<Message> LastMessage(CancellationToken token);
        Task<IList<MessageDto>> GetPagedMessages(int cursorIndex, int pageSize, CancellationToken token);
        Task<IList<MessageDto>> GetPagedMessages(DateTime cursorIndex, int pageSize, CancellationToken token);
    }
}
