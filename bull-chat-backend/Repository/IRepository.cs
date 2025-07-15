namespace bull_chat_backend.Repository
{
    public interface IRepository<T> where T : class
    {
        // CRUD
        // Create
        Task AddAsync(T entity, CancellationToken token);
        Task AddRangeAsync(IEnumerable<T> entities, CancellationToken token);

        // Read
        Task<T> GetByIdAsync(int id, CancellationToken token);
        Task<IEnumerable<T>> GetAllAsync(CancellationToken token);

        // Update
        Task UpdateAsync(T entity, CancellationToken token);

        // Delete
        Task DeleteAsync(T entity, CancellationToken token);
        Task DeleteByIdAsync(int id, CancellationToken token);

        Task<bool> ExistsAsync(int id, CancellationToken token);
        Task<int> CountAsync(CancellationToken token);
    }
}