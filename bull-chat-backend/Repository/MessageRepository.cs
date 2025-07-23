using bull_chat_backend.Models.DBase;
using bull_chat_backend.Models.DTO;
using bull_chat_backend.Repository.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace bull_chat_backend.Repository
{
    public class MessageRepository : IMessageRepository
    {
        private readonly ChatDbContext _context;

        public MessageRepository(ChatDbContext context) => _context = context;

        public async Task AddAsync(Message entity, CancellationToken token)
        {
            _context.User.Attach(entity.User);
            await _context.Message.AddAsync(entity, token);
            await _context.SaveChangesAsync(token);
        }

        public async Task AddRangeAsync(IEnumerable<Message> entities, CancellationToken token)
        {
            await _context.Message.AddRangeAsync(entities);
            await _context.SaveChangesAsync(token);
        }

        public async Task<Message> GetByIdAsync(int id, CancellationToken token)
        {
            return await _context.Message.FindAsync(id, token) ?? Message.Empty;
        }

        public async Task<IEnumerable<Message>> GetAllAsync(CancellationToken token)
        {
            return await _context.Message.ToListAsync(token);
        }

        public async Task UpdateAsync(Message entity, CancellationToken token)
        {
            _context.Message.Update(entity);
            await _context.SaveChangesAsync(token);
        }

        public async Task DeleteAsync(Message entity, CancellationToken token)
        {
            _context.Message.Remove(entity);
            await _context.SaveChangesAsync(token);
        }

        public async Task DeleteByIdAsync(int id, CancellationToken token)
        {
            var message = await _context.Message.FindAsync(id, token);

            if (message != null)
            {
                _context.Message.Remove(message);
                await _context.SaveChangesAsync(token);
            }
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken token)
        {
            return await _context.Message.AnyAsync(m => m.Id == id, token);
        }

        public async Task<int> CountAsync(CancellationToken token)
        {
            return await _context.Message.CountAsync(token);
        }

        public async ValueTask<Message> LastMessage(CancellationToken token)
        {
            var lastMessage = await _context.Message
                .OrderByDescending(m => m.Date)
                .FirstAsync(token);

            return lastMessage;
        }
        public async Task<IList<MessageDto>> GetPagedMessages(DateTime cursor, int pageSize, CancellationToken token)
            => await _context.Message
              .AsNoTracking()
              .Include(m => m.User)
              .Include(m => m.Content)
              .Where(m => m.Date <= cursor)
              .OrderByDescending(m => m)
              .Take(pageSize)
              .OrderBy(m => m)
              .Select(m => m.ToDto())
              .ToListAsync(token);

        public async Task<IList<MessageDto>> GetPagedMessages(int cursorIndex, int pageSize, CancellationToken token)
              => await _context.Message
                .AsNoTracking()
                .Include(m => m.User)
                .Include(m => m.Content)
                .Where(m => m.Id < cursorIndex)
                .OrderByDescending(m => m)
                .Take(pageSize)
                .OrderBy(m => m)
                .Select(m => m.ToDto())
                .ToListAsync(token);
    }
}
