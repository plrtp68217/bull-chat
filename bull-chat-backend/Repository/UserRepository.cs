using bull_chat_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace bull_chat_backend.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ChatDbContext _context;

        public UserRepository(ChatDbContext context) => _context = context;

        public async Task AddAsync(User entity, CancellationToken token)
        {
            await _context.User.AddAsync(entity,token);
            await _context.SaveChangesAsync(token);
        }

        public async Task AddRangeAsync(IEnumerable<User> entities, CancellationToken token)
        {
            await _context.User.AddRangeAsync(entities);
            await _context.SaveChangesAsync(token);
        }

        public async Task<int> CountAsync(CancellationToken token)
        {
            return await _context.User.CountAsync(token);
        }

        public async Task DeleteAsync(User entity, CancellationToken token)
        {
            _context.User.Remove(entity);
            await _context.SaveChangesAsync(token);
        }

        public async Task DeleteByIdAsync(int id, CancellationToken token)
        {
            var user = await _context.User.FindAsync(id, token);
            if (user != null)
            {
                _context.User.Remove(user);
                await _context.SaveChangesAsync(token);
            }
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken token)
        {
            return await _context.User.AnyAsync(u => u.Id == id , token);
        }

        public async Task<IEnumerable<User>> GetAllAsync(CancellationToken token)
        {
            return await _context.User.ToListAsync(token);
        }

        public async Task<User> GetByIdAsync(int id, CancellationToken token) =>
            await _context.User.FindAsync(id, token) ?? User.Empty;

        public async Task UpdateAsync(User entity,CancellationToken token)
        {
            _context.User.Update(entity);
            await _context.SaveChangesAsync(token);
        }

        public async Task<User> GetByNameAsync(string name, CancellationToken token)
        {
            return await _context.User
                .FirstOrDefaultAsync(u => u.Name == name, token) ?? User.Empty;
        }
    }
}