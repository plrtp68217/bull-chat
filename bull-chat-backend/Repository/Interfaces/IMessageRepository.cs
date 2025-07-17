using bull_chat_backend.Models.DBase;

namespace bull_chat_backend.Repository.RepositoryInterfaces
{
    public interface IMessageRepository : IRepository<Message> 
    {
        Task<Message> AddAsync(User user, string content, Models.DBase.Enum.ContentType contentType,CancellationToken token);
    }
}
