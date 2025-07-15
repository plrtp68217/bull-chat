using bull_chat_backend.Models;
using bull_chat_backend.Repository.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace bull_chat_backend.Repository
{
    public class ContentRepository : IContentRepository
    {
        private readonly ChatDbContext _context;

        public ContentRepository(ChatDbContext context) => _context = context;

        public async Task AddAsync(Content entity, CancellationToken token)
        {
            await _context.Content.AddAsync(entity, token);
            await _context.SaveChangesAsync(token);
        }

        public async Task AddRangeAsync(IEnumerable<Content> entities, CancellationToken token)
        {
            await _context.Content.AddRangeAsync(entities);
            await _context.SaveChangesAsync(token);
        }

        public async Task<Content> GetByIdAsync(int id, CancellationToken token)
        {
            return await _context.Content.FindAsync(id, token) ?? Content.Empty;
        }

        public async Task<IEnumerable<Content>> GetAllAsync(CancellationToken token)
        {
            return await _context.Content.ToListAsync(token);
        }

        public async Task UpdateAsync(Content entity, CancellationToken token)
        {
            _context.Content.Update(entity);
            await _context.SaveChangesAsync(token);
        }

        public async Task DeleteAsync(Content entity, CancellationToken token)
        {
            _context.Content.Remove(entity);
            await _context.SaveChangesAsync(token);
        }

        public async Task DeleteByIdAsync(int id, CancellationToken token)
        {
            var content = await _context.Content.FindAsync(id, token);

            if (content != null)
            {
                _context.Content.Remove(content);
                await _context.SaveChangesAsync(token);
            }
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken token)
        {
            return await _context.Content.AnyAsync(c => c.Id == id, token);
        }

        public async Task<int> CountAsync(CancellationToken token)
        {
            return await _context.Content.CountAsync(token);
        }
    }
}
