using bull_chat_backend.Models.DBase;
using bull_chat_backend.Models.DTO;

namespace bull_chat_backend.Repository.RepositoryInterfaces
{
    public interface IMessageRepository : IRepository<Message>
    {
        Task<Message> AddAsync(User user, string content, Models.DBase.Enum.ContentType contentType, CancellationToken token);
        Task<ICollection<MessageDto>> GetDateIntervalAsync(DateTime dateStart, DateTime dateTo);
        Task<ICollection<MessageDto>> GetLastNFromDateAsync(int count, DateTime fromDate);


    }
}
