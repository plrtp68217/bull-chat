using bull_chat_backend.Models.DBase;

namespace bull_chat_backend.Repository.RepositoryInterfaces
{
    public interface IUserRepository : IRepository<User> 
    {
        Task<User> GetByNameAsync(string name, CancellationToken token);
        Task<bool> IsExistByName(string name, CancellationToken token);
    }
}
